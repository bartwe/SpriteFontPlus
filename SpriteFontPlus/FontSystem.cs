using System;
using System.Collections.Generic;

namespace SpriteFontPlus;

sealed unsafe class FontSystem : IDisposable {
    readonly Int32Map<GlyphCollection> _glyphs = new();

    readonly List<Font> _fonts = new();
    int _fontSize;

    public float Spacing;
    public bool UseKernings = true;

    public int? DefaultCharacter = ' ';
    readonly ISpriteService _spriteService;

    public FontSystem(ISpriteService spriteService) {
        _spriteService = spriteService;
    }

    public void Dispose() {
        if (_fonts != null) {
            foreach (var font in _fonts) {
                font.Dispose();
            }
            _fonts.Clear();
        }
        _glyphs?.Clear();
    }

    public void AddFontMem(ReadOnlySpan<byte> data) {
        var font = Font.FromMemory(data);
        font.Recalculate(_fontSize);
        _fonts.Add(font);

        // clear previously unsuccessfully resolved glyphs after loading additional fonts
        foreach (var gc in _glyphs) {
            var oldCache = gc.Value._glyphs;
            var newCache = new Int32Map<FontGlyph?>();
            foreach (var entry in oldCache)
                if (entry.Value != null)
                    newCache[entry.Key] = entry.Value;
            gc.Value._glyphs = newCache;
        }
    }

    GlyphCollection GetGlyphsCollection(int size) {
        if (_glyphs.TryGetValue(size, out var result)) {
            return result;
        }

        result = new();
        _glyphs[size] = result;
        return result;
    }

    public void DrawText(List<GlyphDraw> batch, ReadOnlySpan<char> chars, float scaleX, float scaleY, int fontSize, out int width, out int height) {
        width = 0;
        if (chars.Length == 0) {
            height = 0;
            return;
        }

        if (fontSize != _fontSize) {
            _fontSize = fontSize;
            foreach (var f in _fonts) {
                f.Recalculate(_fontSize);
            }
        }

        var collection = GetGlyphsCollection(_fontSize);

        // Determine ascent and lineHeight from first character
        float ascent = 0, lineHeight = 0;
        for (var i = 0; i < chars.Length; i += StringBuilderIsSurrogatePair(chars, i) ? 2 : 1) {
            var codepoint = StringBuilderConvertToUtf32(chars, i);

            var glyph = GetGlyph(collection, codepoint);
            if (glyph == null) {
                continue;
            }

            ascent = glyph.Font.Ascent;
            lineHeight = glyph.Font.LineHeight;
            break;
        }

        FontGlyphSquad q;

        var originX = 0.0f;
        var originY = 0.0f;

        originY += ascent;

        var lineCount = 1;

        FontGlyph? prevGlyph = null;
        for (var i = 0; i < chars.Length; i += StringBuilderIsSurrogatePair(chars, i) ? 2 : 1) {
            var codepoint = StringBuilderConvertToUtf32(chars, i);

            if (codepoint == '\n') {
                originX = 0.0f;
                originY += lineHeight;
                lineCount++;
                prevGlyph = null;
                continue;
            }

            var glyph = GetGlyph(collection, codepoint);
            if (glyph != null) {
                GetQuad(glyph, prevGlyph, collection, Spacing, ref originX, ref originY, out q);
                // Track width from the rendered quad, not the pen position.
                // This covers glyphs with overhang (negative right-side bearing).
                var scaledRight = (int)(q.X1 * scaleX);
                if (scaledRight > width)
                    width = scaledRight;
                if (glyph.GlyphSprite != null) {
                    q.X0 = (int)(q.X0 * scaleX);
                    q.X1 = (int)(q.X1 * scaleX);
                    q.Y0 = (int)(q.Y0 * scaleY);
                    q.Y1 = (int)(q.Y1 * scaleY);

                    var destRect = new Rectangle((int)q.X0, (int)q.Y0, (int)(q.X1 - q.X0), (int)(q.Y1 - q.Y0));

                    batch.Add(new(destRect, glyph.GlyphSprite!, i));
                }
            }
            prevGlyph = glyph;
        }
        height = (int)((lineCount * lineHeight) * scaleY);
    }


    public void MeasureText(ReadOnlySpan<char> chars, float scaleX, float scaleY, int fontSize, out int width, out int height) {
        width = 0;
        if (chars.Length == 0) {
            height = 0;
            return;
        }

        if (fontSize != _fontSize) {
            _fontSize = fontSize;
            foreach (var f in _fonts) {
                f.Recalculate(_fontSize);
            }
        }

        var collection = GetGlyphsCollection(_fontSize);

        // Determine ascent and lineHeight from first character
        float ascent = 0, lineHeight = 0;
        for (var i = 0; i < chars.Length; i += StringBuilderIsSurrogatePair(chars, i) ? 2 : 1) {
            var codepoint = StringBuilderConvertToUtf32(chars, i);

            var glyph = GetGlyph(collection, codepoint);
            if (glyph == null) {
                continue;
            }

            ascent = glyph.Font.Ascent;
            lineHeight = glyph.Font.LineHeight;
            break;
        }

        var originX = 0.0f;
        var originY = 0.0f;

        originY += ascent;

        var lineCount = 1;

        FontGlyphSquad q;
        FontGlyph? prevGlyph = null;
        for (var i = 0; i < chars.Length; i += StringBuilderIsSurrogatePair(chars, i) ? 2 : 1) {
            var codepoint = StringBuilderConvertToUtf32(chars, i);

            if (codepoint == '\n') {
                originX = 0.0f;
                originY += lineHeight;
                lineCount++;
                prevGlyph = null;
                continue;
            }

            var glyph = GetGlyph(collection, codepoint);
            if (glyph != null) {
                GetQuad(glyph, prevGlyph, collection, Spacing, ref originX, ref originY, out q);
                // Same rule as DrawText: width should match rendered bounds.
                var scaledRight = (int)(q.X1 * scaleX);
                if (scaledRight > width)
                    width = scaledRight;
            }
            prevGlyph = glyph;
        }
        height = (int)((lineCount * lineHeight) * scaleY);
    }

    static bool StringBuilderIsSurrogatePair(ReadOnlySpan<char> chars, int index) {
        if ((index < 0) || (index > chars.Length)) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if ((index + 1) < chars.Length) {
            return char.IsSurrogatePair(chars[index], chars[index + 1]);
        }
        return false;
    }

    static int StringBuilderConvertToUtf32(ReadOnlySpan<char> chars, int index) {
        if ((index < 0) || (index > chars.Length)) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (!char.IsHighSurrogate(chars[index])) {
            return chars[index];
        }

        if (index >= (chars.Length - 1)) {
            throw new("Invalid High Surrogate.");
        }

        return char.ConvertToUtf32(chars[index], chars[index + 1]);
    }

    int GetCodepointIndex(int codepoint, out Font? font) {
        font = null;

        var g = 0;
        foreach (var f in _fonts) {
            g = f.GetGlyphIndex(codepoint);
            if (g != 0) {
                font = f;
                break;
            }
        }

        return g;
    }

    FontGlyph? GetGlyphWithoutBitmap(GlyphCollection collection, int codepoint) {
        if (collection._glyphs.TryGetValue(codepoint, out var glyph)) {
            return glyph;
        }

        var g = GetCodepointIndex(codepoint, out var font);
        if (g == 0) {
            collection._glyphs[codepoint] = null;
            return null;
        }

        int advance, lsb, x0, y0, x1, y1;
        font!.BuildGlyphBitmap(g, font.Scale, &advance, &lsb, &x0, &y0, &x1, &y1);

        var gw = x1 - x0;
        var gh = y1 - y0;

        glyph = new(font, g, gw, gh, (int)(font.Scale * advance * 10.0f), x0, y0);

        collection._glyphs[codepoint] = glyph;

        return glyph;
    }

    FontGlyph? GetGlyphInternal(GlyphCollection glyphs, int codepoint) {
        var glyph = GetGlyphWithoutBitmap(glyphs, codepoint);
        if (glyph == null) {
            return null;
        }

        if ((glyph.Width == 0) || (glyph.Height == 0))
            return glyph;

        if (glyph.GlyphSprite != null) {
            return glyph;
        }

        CreateGlyphSprite(glyph);

        return glyph;
    }

    void CreateGlyphSprite(FontGlyph glyph) {
        var buffer = _spriteService.AllocGlyphBuffer(glyph.Width, glyph.Height);
        glyph.Font.RenderGlyphBitmap((byte*)buffer, glyph.Width, glyph.Height, glyph.Width, glyph.Index);
        glyph.GlyphSprite = _spriteService.RegisterGlyphBuffer(buffer, glyph.Width, glyph.Height);
    }

    FontGlyph? GetGlyph(GlyphCollection glyphs, int codepoint) {
        var result = GetGlyphInternal(glyphs, codepoint);
        if ((result == null) && (DefaultCharacter != null)) {
            result = GetGlyphInternal(glyphs, DefaultCharacter.Value);
        }

        return result;
    }

    void GetQuad(FontGlyph glyph, FontGlyph? prevGlyph, GlyphCollection collection, float spacing, ref float x, ref float y, out FontGlyphSquad q) {
        if (prevGlyph != null) {
            float adv = 0;
            if (UseKernings && (glyph.Font == prevGlyph.Font)) {
                adv = prevGlyph.Font.GetGlyphKernAdvance(prevGlyph.Index, glyph.Index) * glyph.Font.Scale;
            }

            x += (int)(adv + spacing + 0.5f);
        }

        float rx = 0;
        float ry = 0;

        rx = x + glyph.XOffset;
        ry = y + glyph.YOffset;
        q.X0 = rx;
        q.Y0 = ry;
        q.X1 = rx + glyph.Width;
        q.Y1 = ry + glyph.Height;
        x += (int)((glyph.XAdvance / 10.0f) + 0.5f);
    }

    public bool TryGetMissingCharactersInString(ReadOnlySpan<char> chars, List<string> missingCharacterSets, bool includeWhitespace) {
        var i = 0;
        while (i < chars.Length) {
            var isHighSurrogate = char.IsHighSurrogate(chars[i]);

            int codepoint;
            if (isHighSurrogate) {
                if (i == (chars.Length - 1)) {
                    throw new("Encountered high surrogate without low surrogate.");
                }
                if (!char.IsSurrogatePair(chars[i], chars[i + 1])) {
                    throw new("Encountered bad surrogate pair.");
                }
                codepoint = char.ConvertToUtf32(chars[i], chars[i + 1]);
            }
            else {
                codepoint = chars[i];
            }

            var result = GetGlyphWithoutBitmap(GetGlyphsCollection(_fontSize), codepoint);
            if (result == null) {
                var character = chars[i];
                if ((character != '\n') && (character != '\r') && (includeWhitespace || !char.IsWhiteSpace(character))) {
                    if (isHighSurrogate) {
                        missingCharacterSets.Add(new(new[] { chars[i], chars[i + 1] }));
                    }
                    else {
                        missingCharacterSets.Add(new(new[] { chars[i] }));
                    }
                }
            }
            i += isHighSurrogate ? 2 : 1;
        }

        return missingCharacterSets.Count != 0;
    }

    sealed class GlyphCollection {
        internal Int32Map<FontGlyph?> _glyphs = new();
    }
}

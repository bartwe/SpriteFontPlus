using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace SpriteFontPlus;

public sealed class DynamicSpriteFont : IDisposable {
    readonly FontSystem _fontSystem;

    public DynamicSpriteFont(ISpriteService spriteService) {
        _fontSystem = new(spriteService);
    }

    public float Spacing {
        get => _fontSystem.Spacing;
        set => _fontSystem.Spacing = value;
    }

    public bool UseKernings {
        get => _fontSystem.UseKernings;
        set => _fontSystem.UseKernings = value;
    }

    public int? DefaultCharacter {
        get => _fontSystem.DefaultCharacter;
        set => _fontSystem.DefaultCharacter = value;
    }

    public void Dispose() {
        _fontSystem?.Dispose();
    }

    public void DrawString(List<GlyphDraw> batch, ReadOnlySpan<char> text, Vector2 scale, int fontSize, out int width, out int height) {
        _fontSystem.DrawText(batch, text, scale.X, scale.Y, fontSize, out width, out height);
    }

    public void AddTtf(ReadOnlySpan<byte> ttf) {
        _fontSystem.AddFontMem(ttf);
    }

    public void MeasureString(ReadOnlySpan<char> text, Vector2 scale, int fontSize, out int width, out int height) {
        _fontSystem.MeasureText(text, scale.X, scale.Y, fontSize, out width, out height);
    }

    public bool TryGetMissingCharactersInString(ReadOnlySpan<char> text, List<string> missingCharacterSets, bool includeWhitespace) {
        return _fontSystem.TryGetMissingCharactersInString(text, missingCharacterSets, includeWhitespace);
    }
}

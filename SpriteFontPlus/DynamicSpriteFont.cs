using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpriteFontPlus {
    public sealed class DynamicSpriteFont : IDisposable {
        readonly FontSystem _fontSystem;

        public DynamicSpriteFont(ISpriteService spriteService) {
            _fontSystem = new(spriteService);
        }

        public float Spacing {
            get { return _fontSystem.Spacing; }
            set { _fontSystem.Spacing = value; }
        }

        public bool UseKernings {
            get { return _fontSystem.UseKernings; }
            set { _fontSystem.UseKernings = value; }
        }

        public int? DefaultCharacter {
            get { return _fontSystem.DefaultCharacter; }
            set { _fontSystem.DefaultCharacter = value; }
        }

        public void Dispose() {
            _fontSystem?.Dispose();
        }

        public void DrawString(List<GlyphDraw> batch, ReadOnlySpan<char> text, Vector2 scale, int fontSize) {
            _fontSystem.DrawText(batch, text, scale.X, scale.Y, fontSize);
        }

        public void AddTtf(ReadOnlySpan<byte> ttf) {
            _fontSystem.AddFontMem(ttf);
        }

        public Vector2 MeasureString(string text, int fontSize) {
            var bounds = new Bounds();
            _fontSystem.TextBounds(0, 0, text, ref bounds, fontSize);

            return new(bounds.X2, bounds.Y2);
        }

        public Vector2 MeasureString(ReadOnlySpan<char> text, int fontSize) {
            var bounds = new Bounds();
            _fontSystem.TextBounds(0, 0, text, ref bounds, fontSize);

            return new(bounds.X2, bounds.Y2);
        }

        public bool TryGetMissingCharactersInString(string text, List<string> missingCharacterSets, bool includeWhitespace) {
            return _fontSystem.TryGetMissingCharactersInString(text, missingCharacterSets, includeWhitespace);
        }

        public Rectangle GetTextBounds(Vector2 position, string text, int fontSize) {
            var bounds = new Bounds();
            _fontSystem.TextBounds(position.X, position.Y, text, ref bounds, fontSize);

            return new((int)bounds.X, (int)bounds.Y, (int)(bounds.X2 - bounds.X), (int)(bounds.Y2 - bounds.Y));
        }
    }
}

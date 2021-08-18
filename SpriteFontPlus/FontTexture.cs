// unset

using System;

namespace SpriteFontPlus {
    public sealed class FontTexture {
        public int Width;
        public int Height;

        public FontTexture(int width, int height) {
            Width = width;
            Height = height;
        }

        public void SetData(Color[] colorBuffer) {
            throw new NotImplementedException();
        }
    }
}

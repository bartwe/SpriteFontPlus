namespace SpriteFontPlus {
    internal sealed class FontGlyph {
        public Font Font;
        public FontAtlas? Atlas;
        public int Index;
        public Rectangle Bounds;
        public int XAdvance;
        public int XOffset;
        public int YOffset;

        public FontGlyph(Font font, int index, Rectangle bounds, int xAdvance, int xOffset, int yOffset) {
            Font = font;
            Index = index;
            Bounds = bounds;
            XAdvance = xAdvance;
            XOffset = xOffset;
            YOffset = yOffset;
        }

        public static int PadFromBlur(int blur) {
            return blur + 2;
        }
    }
}
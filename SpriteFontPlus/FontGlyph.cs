namespace SpriteFontPlus {
    internal class FontGlyph {
        public Font Font;
        public FontAtlas Atlas;
        public int Index;
        public Rectangle Bounds;
        public int XAdvance;
        public int XOffset;
        public int YOffset;

        public static int PadFromBlur(int blur) {
            return blur + 2;
        }
    }
}
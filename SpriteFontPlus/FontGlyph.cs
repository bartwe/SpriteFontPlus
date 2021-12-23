namespace SpriteFontPlus;

sealed class FontGlyph {
    public Font Font;
    public int Index;
    public int Width;
    public int Height;
    public int XAdvance;
    public int XOffset;
    public int YOffset;
    public GlyphSprite? GlyphSprite;

    public FontGlyph(Font font, int index, int width, int height, int xAdvance, int xOffset, int yOffset) {
        Font = font;
        Index = index;
        Width = width;
        Height = height;
        XAdvance = xAdvance;
        XOffset = xOffset;
        YOffset = yOffset;
    }

    public static int PadFromBlur(int blur) {
        return blur + 2;
    }
}

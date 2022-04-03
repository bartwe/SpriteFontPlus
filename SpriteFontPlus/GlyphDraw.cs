namespace SpriteFontPlus;

public abstract class GlyphSprite { }

public struct GlyphDraw {
    public Rectangle DestRect;
    public GlyphSprite Sprite;
    public int TextIndex;

    public GlyphDraw(Rectangle destRect, GlyphSprite sprite, int textIndex) {
        DestRect = destRect;
        Sprite = sprite;
        TextIndex = textIndex;
    }
}

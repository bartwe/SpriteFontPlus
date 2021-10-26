using System;

namespace SpriteFontPlus {
    public interface ISpriteService {
        IntPtr AllocGlyphBuffer(int width, int height);
        GlyphSprite RegisterGlyphBuffer(IntPtr buffer, int width, int height);
    }
}

using System.Text;

namespace SpriteFontPlus {
    public static class SpriteBatchExtensions {
        public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string _string_, Vector2 pos, Color color, int fontSize) {
            return font.DrawString(batch, _string_, pos, color, fontSize);
        }

        public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string _string_, Vector2 pos, Color color, Vector2 scale, int fontSize) {
            return font.DrawString(batch, _string_, pos, 0f, color, scale, fontSize);
        }

        public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder stringBuilder, Vector2 pos, Color color, int fontSize) {
            return font.DrawString(batch, stringBuilder, pos, color, fontSize);
        }

        public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder stringBuilder, Vector2 pos, Color color, Vector2 scale, int fontSize) {
            return font.DrawString(batch, stringBuilder, pos, 0f, color, scale, fontSize);
        }
    }
}

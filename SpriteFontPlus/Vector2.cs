// unset

namespace SpriteFontPlus {
    public struct Vector2 {
        public static Vector2 One = new() { X = 1, Y = 1 };
        public float X;
        public float Y;

        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }
    }
}
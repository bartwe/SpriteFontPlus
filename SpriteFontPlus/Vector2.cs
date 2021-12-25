// unset

namespace SpriteFontPlus;

public struct Vector2 {
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Vector2 One = new() { X = 1, Y = 1 };
#pragma warning restore CA2211 // Non-constant fields should not be visible
    public float X;
    public float Y;

    public Vector2(float x, float y) {
        X = x;
        Y = y;
    }
}

using SFML.System;

namespace TeleBox.UI;

public struct Vector2
{
    public float x;
    public float y;
    public float Magnitude => (float)Math.Sqrt(x * x + y * y);

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    
    public static Vector2 Zero => new Vector2(0, 0);

    public static Vector2 Lerp(Vector2 start, Vector2 end, float amount)
    {
        var x = Interpolate(start.x, end.x, amount);
        var y = Interpolate(start.y, end.y, amount);

        return new Vector2(x, y);
    }

    private static float Interpolate(float from, float to, float amount)
    {
        return (1.0f - amount) * from + amount * to;
    }
    
    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
    public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
    public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);
}
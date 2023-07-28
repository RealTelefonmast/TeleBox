using System.Runtime.InteropServices;

namespace TeleBox.Engine.Data.Primitive;

[StructLayout(LayoutKind.Explicit)]
public readonly struct Vector2
{
    [FieldOffset(0)]
    public readonly float x;
    [FieldOffset(4)]
    public readonly float y;
    
    public  float LengthSqr =>  x * x + y * y;
    public float Magnitude => (float)Math.Sqrt(LengthSqr);
    public Vector2 Normalized => new Vector2(x / Magnitude, y / Magnitude);
    
    public static Vector2 Zero => new Vector2(0, 0);
    
    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

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
    
    public static Vector2 operator -(Vector2 a) => new Vector2(-a.x, -a.y);
    
    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
    public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
    public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);
    
    public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);
    public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
    
    public static Vector2 Cross(Vector2 a, float s)
    {
        return new Vector2(s * a.y, -s * a.x);
    }
    
        
    public static Vector2 Cross(float s, Vector2 a)
    {
        return new Vector2( -s * a.y, s * a.x );
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }
}
namespace TeleBox.Engine.Data.Primitive;

public struct UIntVec2
{
    public readonly uint x;
    public readonly uint y;

    public static UIntVec2 Zero => new UIntVec2(0, 0);
    
    public UIntVec2(uint x, uint y)
    {
        this.x = x;
        this.y = y;
    }
    
    public static UIntVec2 operator +(UIntVec2 a, UIntVec2 b) => new UIntVec2(a.x + b.x, a.y + b.y);
    public static UIntVec2 operator -(UIntVec2 a, UIntVec2 b) => new UIntVec2(a.x - b.x, a.y - b.y);
    public static UIntVec2 operator *(UIntVec2 a, UIntVec2 b) => new UIntVec2(a.x * b.x, a.y * b.y);
    public static UIntVec2 operator /(UIntVec2 a, UIntVec2 b) => new UIntVec2(a.x / b.x, a.y / b.y);
}
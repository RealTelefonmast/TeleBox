namespace TeleBox.Engine.Data.Primitive;

public struct IntVec2
{
    public readonly int x;
    public readonly int y;
    
    public static implicit operator UIntVec2 (IntVec2 vec) => new UIntVec2((uint)vec.x, (uint)vec.y);
    
    public int Area => x*y;
    public static IntVec2 Zero => new IntVec2(0,0);
    
    public IntVec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public int ToIndex(IntVec2 size)
    {
        return y * size.x + x;
    }

    public static IntVec2 FromIndex(int i, int sizeX)
    {
        int newX = i % sizeX;
        int newZ = i / sizeX;
        return new IntVec2(newX, newZ);
    }

    #region Math

    public static IntVec2 operator +(IntVec2 a, IntVec2 b) => new IntVec2(a.x + b.x, a.y + b.y);
    public static IntVec2 operator -(IntVec2 a, IntVec2 b) => new IntVec2(a.x - b.x, a.y - b.y);
    public static IntVec2 operator *(IntVec2 a, IntVec2 b) => new IntVec2(a.x * b.x, a.y * b.y);
    public static IntVec2 operator /(IntVec2 a, IntVec2 b) => new IntVec2(a.x / b.x, a.y / b.y);

    #endregion
}
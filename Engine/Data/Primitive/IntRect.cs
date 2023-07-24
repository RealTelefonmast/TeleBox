namespace TeleBox.Engine.Data.Primitive;

public class IntRect
{
    public int x;
    public int y;
    public int width;
    public int height;

    public IntVec2 Position => new(x, y);
    public IntVec2 Size => new(width, height);
    
    public int XMax => x + width;
    public int YMax => y + height;
    
    public IntRect(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, width, height);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {width}, {height})";
    }
}
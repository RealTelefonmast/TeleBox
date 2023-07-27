using SFML.System;

namespace TeleBox.Engine.Data.Primitive;

public struct Rect : IEquatable<Rect>
{
    public readonly float x;
    public readonly float y;
    public readonly float width;
    public readonly float height;

    public Vector2f Position => new(x, y);
    public Vector2f Size => new(width, height);
    
    public Rect(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public Rect(Vector2f pos, Vector2f size)
    {
        x = pos.X;
        y = pos.Y;
        width = size.X;
        height = size.Y;
    }

    public bool Contains(IntVec2 point)
    {
        return Contains(point.x, point.y);
    }
    
    public bool Contains(Vector2f point)
    {
        return Contains(point.X, point.Y);
    }

    public bool Contains(float x, float y)
    {
        return x >= this.x && x <= this.x + width && y >= this.y && y <= this.y + height;
    }
    
    public Rect TopPart(float pixels)
    {
        return new Rect(x, y, width, Math.Max(0, pixels));
    }

    public Rect BottomPart(float pixels)
    {
        return new Rect(x, y + height - Math.Max(0, pixels), width, Math.Max(0, pixels));
    }

    public Rect LeftPart(float pixels)
    {
        return new Rect(x, y, Math.Max(0, pixels), height);
    }

    public Rect RightPart(float pixels)
    {
        return new Rect(x + width - Math.Max(0, pixels), y, Math.Max(0, pixels), height);
    }

    public Rect ExpandedBy(float size)
    {
        return new(x - size, y - size, width + size * 2, height + size * 2);
    }

    public Rect ExpandedBy(float x, float y)
    {
        return new(this.x - x, this.y - y, width + x * 2, height + y * 2);
    }

    
    public Rect ContractedBy(float size)
    {
        return ExpandedBy(-size);
    }
    
    public Rect ContractedBy(float x, float y)
    {
        return ExpandedBy(-x, -y);
    }

    public bool Equals(Rect other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && width.Equals(other.width) && height.Equals(other.height);
    }

    public override bool Equals(object? obj)
    {
        return obj is Rect other && Equals(other);
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

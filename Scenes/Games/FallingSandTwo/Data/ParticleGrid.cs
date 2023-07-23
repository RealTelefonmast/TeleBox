using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public static class PixelWorldConfig
{
    internal static int ChunkSize = 512/4;
    internal static float Gravity = 4;
}

public static class GridUtils
{
    public static bool InBounds(this IntVec2 pos, IntVec2 size)
    {
        return pos.x >= 0 && pos.x < size.x && pos.y >= 0 && pos.y < size.y;
    }

    public static bool InBounds(this IntVec2 pos, PixelChunk chunk)
    {
        return pos.x >= 0 && pos.x < chunk.Size.x && pos.y >= 0 && pos.y < chunk.Size.y;
    }
    
    public static int Index(int x, int y, int width)
    {
        return x * width + y;
    }
    
    public static int Index(IntVec2 pos, int width)
    {
        return pos.x * width + pos.y;
    }
    
    public static IntVec2 Position(int index, int width)
    {
        return new IntVec2(index / width, index % width);
    }

    public static bool Inbounds(float x, float y, UIntVec2 size)
    {
        return x >= 0 && x < size.x && y >= 0 && y < size.y;
    }
}


public struct PixelRect
{
    public int x;
    public int y;
    public int width;
    public int height;

    public PixelRect(int i, int i1, int chunkSize, int u)
    {
        x =  i;
        y = i1;
        width = chunkSize;
        height = u;
    }

    public int xMax => x + width;
    public int yMax => y + height;

    public IntVec2 Position => new IntVec2(x, y);
    public IntVec2 Size => new IntVec2(width, height);
    public Rect Rect => new Rect(x,y,width,height); //{ get; set; }
}
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Utility;

public static class GenAdj
{
    public static readonly IntVec2[] AdjacentCells;
    public static IntVec2[] AdjacentCells4Way;

    static GenAdj()
    {
        AdjacentCells = new IntVec2[8];
        AdjacentCells[0] = new IntVec2(0, -1);
        AdjacentCells[1] = new IntVec2(1, -1);
        AdjacentCells[2] = new IntVec2(1, 0);
        AdjacentCells[3] = new IntVec2(1, 1);
        AdjacentCells[4] = new IntVec2(0, 1);
        AdjacentCells[5] = new IntVec2(-1, 1);
        AdjacentCells[6] = new IntVec2(-1, 0);
        AdjacentCells[7] = new IntVec2(-1, -1);
            
        AdjacentCells4Way = new IntVec2[4];
        AdjacentCells4Way[0] = new IntVec2(0, -1);
        AdjacentCells4Way[1] = new IntVec2(1, 0);
        AdjacentCells4Way[2] = new IntVec2(0, 1);
        AdjacentCells4Way[3] = new IntVec2(-1, 0);
        
    }

    public static IntVec2 Down => new IntVec2(0, 1);
    public static IntVec2 Up => new IntVec2(0, -1);
    public static IntVec2 Left => new IntVec2(-1, 0);
    public static IntVec2 Right => new IntVec2(1, 0);
    public static IntVec2 DownLeft => new IntVec2(-1, 1);
    public static IntVec2 DownRight => new IntVec2(1, 1);
}
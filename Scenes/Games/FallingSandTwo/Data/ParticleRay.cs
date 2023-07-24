using TeleBox.Engine.Data.Primitive;
using TeleBox.Scenes.Games.FallingSandTwo.Data.World;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public struct ParticleRay
{
    private IntVec2 from;
    private IntVec2 to;
    
    public ParticleRay(IntVec2 from, IntVec2 to, ParticleGrid grid)
    {
        this.from = from;
        this.to = to;
        RayCast(from, to, grid);
    }

    public bool IsClear { get; set; } = true;
    public IntVec2? HitSolid { get; set; }
    public IntVec2? HitLiquid { get; set; }
    public IntVec2? HitGas { get; set; }

    private void RayCast(IntVec2 from, IntVec2 to, ParticleGrid grid)
    {
        int x0 = from.x, 
            y0 = from.y, 
            x1 = to.x, 
            y1 = to.y;
        
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var x = x0;
        var y = y0;
        var n = 1 + dx + dy;
        var x_inc = (x1 > x0) ? 1 : -1;
        var y_inc = (y1 > y0) ? 1 : -1;
        var error = dx - dy;
        dx *= 2;
        dy *= 2;

        var xPrev = x;
        var yPrev = y;
        
        for (; n > 0; --n)
        {
            //Check Pos
            var index = y * grid.Size.x + x;
            if (!PixelFunctions.Inbounds(index, grid))
            {
                HitSolid = new IntVec2(xPrev, yPrev);
                IsClear = false;
                return;
            }
            if (grid[index].id == MaterialType.Sand)
            {
                HitSolid = new IntVec2(xPrev, yPrev);
                IsClear = false;
            }
            else if (grid[index].id == MaterialType.Water)
            {
                HitLiquid = new IntVec2(x, y);
                IsClear = false;
            }
            else if (grid[index].id == MaterialType.Smoke)
            {
                HitGas = new IntVec2(x, y);
                IsClear = false;
            }
            
            //
            if (error > 0)
            {
                xPrev = x;
                x += x_inc;
                error -= dy;
            }
            else
            {
                yPrev = y;
                y += y_inc;
                error += dx;
            }
        }
    }
}
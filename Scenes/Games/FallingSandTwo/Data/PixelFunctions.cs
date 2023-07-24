using System.Runtime.InteropServices;
using SFML.Graphics;
using TeleBox.Engine.Data;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Games.FallingSandTwo.Data.Functions;
using TeleBox.UI;
using TerraFX.Interop.Windows;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

/*
[StructLayout(LayoutKind.Sequential)]
public struct ParticleState
{
    //Center
    public readonly Particle Self;
    
    public readonly Particle Top;
    public readonly Particle TopRight;
    public readonly Particle Right;
    public readonly Particle BottomRight;
    public readonly Particle Bottom;
    public readonly Particle BottomLeft;
    public readonly Particle Left;
    public readonly Particle TopLeft;
    
    public bool IsInLiquid => Top.id == MaterialType.Water || TopRight.id == MaterialType.Water || Right.id == MaterialType.Water || BottomRight.id == MaterialType.Water || Bottom.id == MaterialType.Water || BottomLeft.id == MaterialType.Water || Left.id == MaterialType.Water || TopLeft.id == MaterialType.Water;

    public ParticleState(IntVec2 pos, Particle particle, PixelChunk chunk)
    {
        Self = particle;
        var topPos = pos + GenAdj.AdjacentCells[0];
        var topRightPos = pos + GenAdj.AdjacentCells[1];
        var rightPos = pos + GenAdj.AdjacentCells[2];
        var bottomRightPos = pos + GenAdj.AdjacentCells[3];
        var bottomPos = pos + GenAdj.AdjacentCells[4];
        var bottomLeftPos = pos + GenAdj.AdjacentCells[5];
        var leftPos = pos + GenAdj.AdjacentCells[6];
        var topLeftPos = pos + GenAdj.AdjacentCells[7];
        
        if(topPos.InBounds(chunk))
            Top = chunk[topPos];
        if(topRightPos.InBounds(chunk))
            TopRight = chunk[topRightPos];
        if(rightPos.InBounds(chunk))
            Right = chunk[rightPos];
        if(bottomRightPos.InBounds(chunk))
            BottomRight = chunk[bottomRightPos];
        
        if(bottomPos.InBounds(chunk))
            Bottom = chunk[bottomPos];
        if(bottomLeftPos.InBounds(chunk))
            BottomLeft = chunk[bottomLeftPos];
        if(leftPos.InBounds(chunk))
            Left = chunk[leftPos];
        if(topLeftPos.InBounds(chunk))
            TopLeft = chunk[topLeftPos];
        
    }
}

public static class StateSolver
{
    public static void SolveSand(IntVec2 pos, Particle particle, PixelChunk chunk)
    {
        var state = new ParticleState(pos, particle, chunk);
    }
}
*/

public static class PixelFunctions
{
    public static void Update(int index, IntVec2 pos, Particle particle, ParticleGrid grid)
    {
        if (particle.hasBeenUpdated)
            return;
        
        switch (particle.id)
        {
            case MaterialType.Empty:
                break;
            case MaterialType.Sand:
                UpdateSand(index, pos, particle, grid);
                break;
            case MaterialType.Water:
                //UpdateWater(index, pos, particle, grid);
                break;

            case MaterialType.Wanderer:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region Cell Checks

    internal static bool Inbounds(IntVec2 pos, ParticleGrid grid)
    {
        return pos.x >= 0 && pos.x < grid.Size.x && pos.y >= 0 && pos.y < grid.Size.y;
    }
    
    internal static bool IsEmpty(IntVec2 pos, ParticleGrid grid)
    {
        if (Inbounds(pos, grid))
        {
            return grid[pos].id == MaterialType.Empty;
        }
        return false;
    }

    internal static bool IsInLiquid(IntVec2 pos, ParticleGrid grid, out IntVec2 lPos)
    {
        lPos = IntVec2.Zero;
        for (var i = 0; i < 8; i++)
        {
            var adj = pos + GenAdj.AdjacentCells[i];
            var nPos = new IntVec2(adj.x, adj.y);
            if (!Inbounds(lPos, grid) || grid[lPos].id != MaterialType.Water) continue;
            lPos = nPos;
            return true;
        }

        return false;
    }
    
    #endregion
    
    public static void UpdateSand(int index, IntVec2 pos, Particle particle, ParticleGrid grid)
    {
        //TODO: EDGE CASE: horizontal or vertical rows of sand start cascading:
        //When a particle next to another sand particle falls down, and cannot move left or down, it moves to the below right
        //Below the neighbor particle, this particle then cannot move down or to the left either, so it moves right next to the other particle
        //This makes it look like the particles all move in sync to the right

        _ = new ParticleFunction(pos, particle).MoveDown(grid).MoveDownLeft(grid).MoveDownRight(grid);
    }


    /*public static void UpdateWater(int index, IntVec2 pos, Particle particle, ParticleGrid grid)
    {
        var pos = new IntVec2(x, y);
        var belowPos = pos + GenAdj.Down;
        var leftPos = pos + GenAdj.Left;
        var rightPos = pos + GenAdj.Right;

        if (InBoundsOrNeighbor(belowPos, chunk))
        {
            if (IsEmpty(belowPos, chunk))
            {
                chunk.Set(belowPos, p, true);
                chunk.Set(pos, Particle.Empty, true);
            }
            else
            {
                var moveDir = Rand.Bool ? 1 : -1;
                var first = moveDir == 1 ? rightPos : leftPos;
                var second = moveDir == 1 ? leftPos : rightPos;
                if (InBoundsOrNeighbor(first, chunk) && IsEmpty(first, chunk))
                {
                    chunk.Set(first, p, true);
                    chunk.Set(pos, Particle.Empty, true);
                }
                else if(InBoundsOrNeighbor(second, chunk) && IsEmpty(second, chunk))
                {
                    chunk.Set(second, p, true);
                    chunk.Set(pos, Particle.Empty, true);
                }
            }
        }
    }

    public static void UpdateWater2(PixelChunk chunk, int x, int y, int index, Particle p)
    {
        float dt = 1;
        var write_idx = index;

        int spreadRate = 5;
        int fallRate = 2;

        //var state = new ParticleState(new(x, y), p, chunk);

        p.velocity += new Vector2(0, GenMath.Clamp(PixelWorldConfig.Gravity * 1/60f, -10, 10));

        if (InBoundsOrNeighbor(new(x, y + 1), chunk) && 
            (!IsEmpty(new(x, y + 1), chunk)))
        {
            p.velocity /= new Vector2(1, 2);
        }

        p.hasBeenUpdated = true;

        //
        var randomB = Rand.Bool;
        var right = randomB ? spreadRate : -spreadRate;
        var left = -right;
        var v_idx = GridUtils.Index((int) (x + p.velocity.x), (int) (y + p.velocity.y), chunk.Size.x);
        var b_idx = GridUtils.Index(x, y + fallRate, chunk.Size.x);
        var bl_idx = GridUtils.Index(x + left, y + fallRate, chunk.Size.x);
        var br_idx = GridUtils.Index(x + right, y + fallRate, chunk.Size.x);
        var l_idx = GridUtils.Index(x + left, y, chunk.Size.x);
        var r_idx = GridUtils.Index(x + right, y, chunk.Size.x);
        var vx = (int) p.velocity.x;
        var vy = (int) p.velocity.y;
        int lx, ly;

        if (InBoundsOrNeighbor(new(x + vx, y + vy), chunk) && 
            (IsEmpty(new(x + vx, y + vy), chunk)))
        {
            chunk.Set(v_idx, p, true);
            chunk.Set(index, Particle.Empty, true);
        }
        else if (InBoundsOrNeighbor(new(x, y + fallRate), chunk) && 
                 IsEmpty(new(x, y + fallRate), chunk))
        {
            chunk.Set(b_idx, p, true);
            chunk.Set(index, Particle.Empty, true);
        }
        else if (InBoundsOrNeighbor(new(x + right, y + fallRate), chunk) && 
                 IsEmpty(new(x + right, y + fallRate), chunk))
        {
            chunk.Set(br_idx, p, true);
            chunk.Set(index, Particle.Empty, true);
        }
        else if (InBoundsOrNeighbor(new(x + left, y + fallRate), chunk)&& 
                 IsEmpty(new(x + left, y + fallRate), chunk))
        {
            chunk.Set(bl_idx, p, true);
            chunk.Set(index, Particle.Empty, true);
        }
        //
        else if (InBoundsOrNeighbor(new IntVec2(x, y + fallRate), chunk) && 
                 IsEmpty(new IntVec2(x, y + fallRate), chunk)) 
        {
            p.velocity += new Vector2(0, PixelWorldConfig.Gravity * dt);
            var tmp_b = chunk[new IntVec2(x, y + fallRate)];
            
            chunk.Set(b_idx, p, true);
            chunk.Set(index, tmp_b, true);
        }
        else if (InBoundsOrNeighbor(new IntVec2(x + left, y + fallRate), chunk) &&
                 IsEmpty(new IntVec2(x + left, y + fallRate), chunk))
        {
            p.velocity +=new Vector2(IsInLiquid(new IntVec2(x, y), chunk, out var lPos) ? 0 : Rand.Bool ? -1 : 1, PixelWorldConfig.Gravity * dt);
            var tmp_b = chunk[new IntVec2(x + left, y + fallRate)];
            chunk.Set(bl_idx, p, true);
            chunk.Set(index, tmp_b, true);
        }
        else if (InBoundsOrNeighbor(new IntVec2(x + right, y + fallRate), chunk) &&
                 IsEmpty(new IntVec2(x + right, y + fallRate), chunk))
        {
            p.velocity +=new Vector2(IsInLiquid(new IntVec2(x, y), chunk, out var lPos) ? 0 : Rand.Bool ? -1 : 1,PixelWorldConfig.Gravity * dt);
            var tmp_b = chunk[new IntVec2(x + right, y + fallRate)];
            chunk.Set(br_idx, p, true);
            chunk.Set(index, tmp_b, true);
        }
        // else if (IsInLiquid(new IntVec2(x, y), chunk, out var lPos) && Rand.Range(0, 10) == 0)
        // {
        //     var tmp_b = chunk[lPos];
        //     chunk.Set(lPos, p, true);
        //     chunk.Set(index, tmp_b, true);
        // }
        else 
        {
            var tmp = p;
            var found = false;

            // Don't try to spread if something is directly above you?
            if (IsSurrounded(new IntVec2(x, y), chunk)) {
                chunk.Set(index, tmp, false);
                return;	
            }
            else 
            {
                for (int i = 0; i < fallRate; ++i)
                {
                    for (int j = spreadRate; j > 0; --j)
                    {
                        if (InBoundsOrNeighbor(new(x - j, y + i), chunk) &&
                            (IsEmpty(new(x - j, y + i), chunk)))
                        {
                            var tmp_b = chunk[x - j, y + i];
                            chunk.Set(new IntVec2(x - j, y + i), p, true);
                            chunk.Set(index, tmp_b, true);
                            found = true;
                            break;
                        }

                        if (InBoundsOrNeighbor(new(x + j, y + i), chunk) &&
                            (IsEmpty(new(x + j, y + i), chunk)))
                        {
                            var tmp_b = chunk[x + j, y + i];
                            chunk.Set(new IntVec2(x + j, y + i), p, true);
                            chunk.Set(index, tmp_b, true);
                            found = true;
                            break;
                        }
                    }
                }

                // if (!found)
                // {
                //     chunk.Set(index, tmp, true);
                // }
            }
        }
    }*/
}


    
    

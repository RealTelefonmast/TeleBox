using TeleBox.Engine.Data;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Games.FallingSandTwo.Data.World;
using TeleBox.Scenes.Materials;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data.Functions;

public struct ParticleFunction
{
    private readonly IntVec2 _pos;
    private Particle _p;
    private bool _completed = false;

    public bool IsCompleted => _completed;

    public static ParticleFunction Completed => new ParticleFunction()
    {
        _completed = true
    };

    public ParticleFunction(IntVec2 pos, Particle p)
    {
        _pos = pos;
        _p = p;
    }

    public ParticleFunction HandleRayCast(IntVec2 desired, ParticleGrid grid)
    {
        var ray = new ParticleRay(_pos, desired, grid);
        if (ray.IsClear)
        {
            if (TrySwap(_pos, desired, _p, grid))
            {
                return Completed;
            }

            return Completed;
        }
        
        //
        if (ray.HitSolid.HasValue)
        {
            var hit = ray.HitSolid.Value;
            if (TrySwap(_pos, hit, _p, grid))
            {
                return Completed;
            }
        }
        else if (ray.HitLiquid.HasValue)
        {
            var hit = ray.HitLiquid.Value;
            if (TrySwap(_pos, hit, _p, grid))
            {
                return Completed;
            }
        }
        else if (ray.HitGas.HasValue)
        {
            var hit = ray.HitGas.Value;
            if (TrySwap(_pos, hit, _p, grid))
            {
                return Completed;
            }
        }

        return this;
    }
    
    public ParticleFunction Accelerate(float xAcc, float yAcc, ParticleGrid grid)
    {
        _p.velocity += new Vector2(xAcc, yAcc);
        grid.Set(_pos, _p, true);
        return this;
    }
    
    public ParticleFunction MoveByAcc(ParticleGrid grid)
    {
        var desired = _pos + new IntVec2((int)_p.velocity.x, (int)_p.velocity.y);
        return HandleRayCast(desired, grid);
    }

    public ParticleFunction Spread(ParticleGrid grid)
    {   
        var spreadRate = Rand.Bool ? 1 : -1;
        return HandleRayCast(_pos + new IntVec2(spreadRate, 1), grid);
    }

    public ParticleFunction MoveDownLeftRight(ParticleGrid grid)
    {
        var cellBelow = new IntVec2(Rand.Chance(0.5f) ? (_pos.x + 1) : (_pos.x - 1), _pos.y + 1);
        if (TrySwap(_pos, cellBelow, _p, grid))
        {
            return Completed;
        }
        return this;
    }

    public ParticleFunction MoveInLiquid(ParticleGrid grid)
    {
        if (Rand.Chance(0.1f) && PixelFunctions.NearLiquid(_pos, grid, out var lPos))
        {
            if (TrySwap(_pos, lPos, _p, grid))
            {
                return Completed;
            }
        }
        return this;
    }
    
    public ParticleFunction MoveDown(ParticleGrid grid)
    {
        if (_completed) return this;
        if (TrySwap(_pos, _pos + GenAdj.Down, _p, grid))
        {
            return Completed;
        }

        return this;
    }

    public ParticleFunction RandLeftRight(ParticleGrid grid)
    {
        if (_completed) return this;
        return Rand.Bool ? MoveDownLeft(grid).MoveDownRight(grid) : MoveDownRight(grid).MoveDownLeft(grid);
    }

    public ParticleFunction MoveDownLeft(ParticleGrid grid)
    {
        if (_completed) return this;
        if (TrySwap(_pos, _pos + GenAdj.DownLeft, _p, grid))
        {
            return Completed;
        }

        return this;
    }

    public ParticleFunction MoveDownRight(ParticleGrid grid)
    {
        if (_completed) return this;
        if (TrySwap(_pos, _pos + GenAdj.DownRight, _p, grid))
        {
            return Completed;
        }

        return this;
    }
    
    public ParticleFunction Set(ParticleGrid grid)
    {
        grid.Set(_pos, _p, true);
        return this;
    }

    private static bool TrySwap(IntVec2 oldPos, IntVec2 newPos, Particle p, ParticleGrid grid)
    {
        if (PixelFunctions.Inbounds(newPos, grid) && PixelFunctions.IsEmpty(newPos, grid))
        {
            var temp = grid[newPos];
            grid.Set(newPos, p, true);
            grid.Set(oldPos, temp, false);
            return true;
        }

        return false;
    }
}
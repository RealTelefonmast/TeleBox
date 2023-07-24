using TeleBox.Engine.Data;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data.Functions;

public struct ParticleFunction
{
    private readonly IntVec2 _pos;
    private readonly Particle _p;
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

    public ParticleFunction MoveDown(ParticleGrid grid)
    {
        if (_completed) return this;
        if (Swap(_pos, _pos + GenAdj.Down, _p, grid))
        {
            return Completed;
        }

        return this;
    }

    public ParticleFunction MoveDownLeft(ParticleGrid grid)
    {
        if (_completed) return this;
        if (Swap(_pos, _pos + GenAdj.DownLeft, _p, grid))
        {
            return Completed;
        }

        return this;
    }

    public ParticleFunction MoveDownRight(ParticleGrid grid)
    {
        if (_completed) return this;
        if (Swap(_pos, _pos + GenAdj.DownRight, _p, grid))
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

    private static bool Swap(IntVec2 oldPos, IntVec2 newPos, Particle p, ParticleGrid grid)
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
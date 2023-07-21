using TeleBox.Engine.Utility;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public static class PixelWorldConfig
{
    internal static uint ChunkSize = 512;
    internal static float Gravity = 4;
}

public static class GridUtils
{
    public static uint Index(uint x, uint y, uint width)
    {
        return x * width + y;
    }

    public static UIntVec2 Position(uint index, uint width)
    {
        return new UIntVec2(index / width, index % width);
    }

    public static bool Inbounds(float x, float y, UIntVec2 size)
    {
        return x >= 0 && x < size.x && y >= 0 && y < size.y;
    }

    public static bool Inbounds(uint index, int length)
    {
        return index < length;
    }
    
    public static uint Constrained(uint desiredValue, uint maxValue, uint currentValue)
    {
        return currentValue + desiredValue > maxValue ? maxValue - currentValue : desiredValue;
    }
}


public struct PixelRect
{
    public uint x;
    public uint y;
    public uint width;
    public uint height;

    public UIntVec2 Position => new UIntVec2(x, y);
    public UIntVec2 Size => new UIntVec2(width, height);
}

public static class PixelFunctions
{
    public static void Update(PixelChunk chunk, uint x, uint y, uint index, Particle particle)
    {
        switch (particle.id)
        {
            case MaterialType.Empty:
                break;
            case MaterialType.Sand:
                UpdateSand(chunk, x, y, index, particle);
                break;
            case MaterialType.Water:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static bool IsEmpty(PixelChunk chunk, uint x, uint y)
    {
        var part = chunk[x, y];
        return part.id == MaterialType.Empty;
    }

    public static void UpdateSand(PixelChunk chunk, uint x, uint y, uint index, Particle p)
    {
        float dt = 1 / 60f; //TODO:Current.DeltaTime;
        int fallRate = 4;

        var clampY = GenMath.Clamp(p.velocity.y + (PixelWorldConfig.Gravity * dt), -10, 10);
        p.velocity = new Vector2(p.velocity.x, clampY);
        if (GridUtils.Inbounds(x, y+1, chunk.Size) && !IsEmpty(chunk, x, y + 1))
        {
            p.velocity = new Vector2(p.velocity.x, p.velocity.y / 2);
        }
        
        var vi_x = x + p.velocity.x;
        var vi_y = y + p.velocity.y;

        var velocityTarget = new UIntVec2((uint)vi_x, (uint)vi_y);
        var velocityIndex = GridUtils.Index(velocityTarget.x, velocityTarget.y, chunk.Size.x);

        uint below = GridUtils.Index(x, y + 1, chunk.Size.x);
        uint belowRight = GridUtils.Index(x + 1, y + 1, chunk.Size.x);
        uint belowLeft = GridUtils.Index(x - 1, y + 1, chunk.Size.x);
        
        var tmp_a = chunk[index];
        //Physics
        /*var shouldAttemptOther = true;
        if (GridUtils.Inbounds(velocityIndex, chunk.Length) && IsEmpty(chunk, velocityTarget.x, velocityTarget.y))
        {
            var targetParticle = chunk[velocityIndex];
            if(!targetParticle.hasBeenUpdated && targetParticle.velocity.Magnitude - p.velocity.Magnitude > 10)
            {
                chunk.Set(velocityIndex, p);
                chunk.Set(index, targetParticle);
                shouldAttemptOther = false;
            }
        }

        if (shouldAttemptOther)
        {*/
        
        // if(velocityIndex == index && IsEmpty(chunk, x, y + 1))
        // {
        //     chunk.Set(index, p);
        //     return;
        // }        
        
        if (GridUtils.Inbounds(x, y + 1, chunk.Size) && IsEmpty(chunk, x, y + 1))
        {
            p.velocity = new Vector2(p.velocity.x, PixelWorldConfig.Gravity * dt);
            var temp = chunk[below];
            chunk.Set(below, p);
            chunk.Set(index, temp);
        }
        else if (GridUtils.Inbounds(x - 1, y + 1, chunk.Size) && IsEmpty(chunk, x - 1, y + 1))
        {
            p.velocity = new Vector2(Rand.Range(0, 1) == 0 ? -1 : 1, PixelWorldConfig.Gravity * dt);
            var temp = chunk[x - 1, y - 1];
            chunk.Set(belowLeft, p);
            chunk.Set(index, temp);
        }
        else if (GridUtils.Inbounds(x + 1, y + 1, chunk.Size) && IsEmpty(chunk, x + 1, y + 1))
        {
            p.velocity = new Vector2(Rand.Range(0, 1) == 0 ? -1 : 1, PixelWorldConfig.Gravity * dt);
            var temp = chunk[x + 1, y + 1];
            chunk.Set(belowRight, p);
            chunk.Set(index, temp);
        }
    }
}
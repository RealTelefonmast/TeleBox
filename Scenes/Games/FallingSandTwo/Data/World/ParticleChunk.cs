using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data.World;

public struct ParticleChunk
{
    private IntRect _bounds;
    private IntRect _dirtyRect;

    public IntRect Bounds => _bounds;
    public IntRect DirtyRect => _dirtyRect;

    public ParticleChunk(IntRect bounds)
    {
        _bounds = bounds;
        _dirtyRect = new IntRect(0, 0, 0, 0);
    }

    public void SetDirty(IntRect rect)
    {
        _dirtyRect = rect;
    }
}
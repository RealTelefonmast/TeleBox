using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public class PixelChunk
{
    private PixelRect _bounds;
    private List<PixelRect> _dirtyRects;
    private Particle[] _particles;

    public Particle this[uint index] => _particles[index];

    public Particle this[uint x, uint y] => _particles[GridUtils.Index(x, y, _bounds.width)];

    public PixelChunk(PixelRect rect)
    {
        _bounds = rect;
        _dirtyRects = new List<PixelRect>();
        _particles = new Particle[PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize];
    }

    public UIntVec2 Size => _bounds.Size;
    public int Length => _particles.Length;
    public Rect Bounds  => new Rect(_bounds.x, _bounds.y, _bounds.width, _bounds.height);

    public void Update(float delta)
    {
        for (uint x = 0; x < _bounds.width; x++)
        {
            for(uint y = 0; y < _bounds.height; y++)
            {
                var index = GridUtils.Index(x, y, _bounds.width);
                var particle = _particles[index];
                PixelFunctions.Update(this, x, y, index, particle);
                
            }
        }
        
        //Reset
        for (uint x = 0; x < _bounds.width; x++)
        {
            for(uint y = 0; y < _bounds.height; y++)
            {
                _particles[GridUtils.Index(x, y, _bounds.width)].hasBeenUpdated = false;
            }
        }
    }

    public void Set(uint x, uint y, Particle particle)
    {
        Set(GridUtils.Index(x, y, _bounds.width), particle);
    }

    public void Set(uint index, Particle particle)
    {
        //Write Particle
        _particles[index] = particle;
        //Set Pixel Color
        var vec = GridUtils.Position(index, Size.x);
        FS_Current.World.SetPixel((int)vec.x, (int)vec.y, particle.color);

    }
}
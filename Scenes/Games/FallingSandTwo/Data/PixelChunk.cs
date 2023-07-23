using SFML.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.UI;
using TerraFX.Interop.Windows;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

//TODO: 
public class PixelChunk
{
    private readonly int _id;
    private PixelRect _bounds;
    private List<PixelRect> _dirtyRects;
    private Particle[] _particles;
    private PixelChunk[] _neighbors;
    
    public Particle this[int index] => _particles[index];
    public Particle this[IntVec2 pos] => _particles[GridUtils.Index(pos.x, pos.y, _bounds.width)];
    public Particle this[int x, int y] => _particles[GridUtils.Index(x, y, _bounds.width)];

    public PixelChunk(int id, PixelRect rect)
    {
        _id = id;
        _bounds  = rect;
        _neighbors = new PixelChunk[4];
        _dirtyRects = new List<PixelRect>();
        _particles = new Particle[PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize];
        DirtyRect = new PixelRect(0, 0, PixelWorldConfig.ChunkSize, PixelWorldConfig.ChunkSize);
    }

    public void SetNeighbor(PixelChunk chunk, int index)
    {
        _neighbors[index] = chunk;
    }
    
    public int ID => _id;
    public IntVec2 Size => _bounds.Size;
    public int Length => _particles.Length;
    public Rect Bounds  => new Rect(_bounds.x, _bounds.y, _bounds.width, _bounds.height);

    public PixelRect DirtyRect { get; set; }

    private uint Frame { get; set; }

    public IntVec2 wandering = new IntVec2(0, 0);

    public void Update(float delta)
    {
        
        //Update dirty rect
        for (var x = DirtyRect.x; x < DirtyRect.xMax; x++)
        {
            for(var y = DirtyRect.y; y < DirtyRect.yMax; y++)
            {
                var index = GridUtils.Index(x, y, _bounds.width);
                var particle = _particles[index];
                if (particle.id != MaterialType.Empty)
                {
                    PixelFunctions.Update(this, x, y, index, particle);
                }
            }
        }
        
        //Reset
        DirtyRect = new PixelRect(0, 0, PixelWorldConfig.ChunkSize,  PixelWorldConfig.ChunkSize);
        var workingRect = new PixelRect( PixelWorldConfig.ChunkSize,  PixelWorldConfig.ChunkSize, 0, 0);
        for (var x = 0; x < _bounds.width; x++)
        {
            for (var y = 0; y < _bounds.height; y++)
            {
                var index = GridUtils.Index(x, y, _bounds.width);
                var particle = _particles[index];
                if (!particle.hasBeenUpdated)
                {
                    continue;
                }

                const int padding = 8;
                const int paddingHalf = padding / 2;

                if (x < workingRect.x)
                {
                    var tmp = workingRect;
                    tmp.x = Math.Clamp(x - paddingHalf, 0, PixelWorldConfig.ChunkSize - 1);
                    DirtyRect = workingRect = tmp;
                }

                if (y < workingRect.y)
                {
                    var tmp = workingRect;
                    tmp.y = Math.Clamp(y - paddingHalf, 0, PixelWorldConfig.ChunkSize - 1);
                    DirtyRect = workingRect = tmp;
                }

                if ((x + padding) > workingRect.xMax)
                {
                    var tmp = workingRect;
                    var width = (x - workingRect.x) + padding;
                    tmp.width = Math.Clamp(width, 0, PixelWorldConfig.ChunkSize - (workingRect.x));
                    DirtyRect = workingRect = tmp;
                }

                if ((y + padding) > workingRect.yMax)
                {
                    var tmp = workingRect;
                    var height = (y - workingRect.y) + padding;
                    tmp.height = Math.Clamp(height, 0, PixelWorldConfig.ChunkSize - (workingRect.y));
                    DirtyRect = workingRect = tmp;
                }

                particle.hasBeenUpdated = false;
                _particles[GridUtils.Index(x, y, _bounds.width)] = particle;

            }
        }

        //
        Frame++;
    }

    public void Set(int x, int y, Particle particle)
    {
        Set(GridUtils.Index(x, y, _bounds.width), particle);
    }

    public void Set(IntVec2 pos, Particle particle, bool update, PixelChunk ignore = null)
    {
        if (IsInNeighbor(pos, out var nghb, out var nghbPos, ignore))
        {
            nghb.Set(nghbPos, particle, update, this);
            return;
        }
        Set(GridUtils.Index(pos, _bounds.width), particle, update);
    }

    public bool IsInNeighbor(int index, out PixelChunk neighbor, out int nghbIndex, PixelChunk toIgnore = null)
    {
        nghbIndex = 0;
        var ye = IsInNeighbor(GridUtils.Position(index, Size.x), out neighbor, out var pos, toIgnore);
        if(ye)
            nghbIndex = GridUtils.Index(pos, neighbor.Size.x);
        return ye;
    }

    public bool IsInNeighbor(IntVec2 pos, out PixelChunk neighbor, out IntVec2 neighborPos, PixelChunk toIgnore = null)
    {
        neighbor = null;
        neighborPos = IntVec2.Zero;
        if (pos.x >= PixelWorldConfig.ChunkSize && _neighbors[1] != null)
        {
            neighbor = _neighbors[1];
            if (toIgnore == neighbor) return false;
            neighborPos = new IntVec2(pos.x-_bounds.width, pos.y);
            return true;
        }
        
        if (pos.x < 0 && _neighbors[3] != null)
        {
            neighbor = _neighbors[3];
            
            if (toIgnore == neighbor) return false;
            neighborPos = new IntVec2(pos.x+_bounds.width, pos.y);
            return true;
        }
        
        if (pos.y >= PixelWorldConfig.ChunkSize && _neighbors[2] != null)
        {
            neighbor = _neighbors[2];
            
            if (toIgnore == neighbor) return false;
            neighborPos = new IntVec2(pos.x, pos.y-_bounds.height);
            return true;
        }

        if (pos.y < 0 && _neighbors[0] != null)
        {
            neighbor = _neighbors[0];
            
            if (toIgnore == neighbor) return false;
            neighborPos = new IntVec2(pos.x, pos.y+_bounds.height);
            return true;
        }
        
        return false;

    }
    
    public void Set(int index, Particle particle, bool update = false, PixelChunk ignore = null)
    {
        if (IsInNeighbor(index, out var nghb, out var nghbIndex, ignore))
        {
            nghb.Set(nghbIndex, particle, update, this);
            return;
        }
        
        //Write Particle
        if(update)
            particle.hasBeenUpdated = true;
        _particles[index] = particle;
        
        //Set Pixel Color
        var vec = GridUtils.Position(index, Size.x);
        var pos= _bounds.Position + vec;
        FS_Current.World.SetPixel((int)pos.x, (int)pos.y, particle.color);
    }

    public void DrawDebug()
    {
        Widgets.Rectangle(new Rect(_bounds.x + DirtyRect.x,
            _bounds.y + DirtyRect.y, DirtyRect.width, DirtyRect.height) , Color.Red, Color.Transparent);
    }
}
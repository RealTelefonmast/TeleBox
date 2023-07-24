using SFML.Graphics;
using SFML.Window;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;
using IntRect = TeleBox.Engine.Data.Primitive.IntRect;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data.World;

public class ParticleGrid
{
    //Main Buffer
    private IntVec2 _size;
    private IntVec2 _chunkSize;
    private readonly Particle[] _particles;
    private readonly ParticleChunk[] _dataChunks;

    private static int ChunkSize => PixelWorldConfig.ChunkSize;
    public IntVec2 Size => _size;

    
    public Particle this[int index] => _particles[index];
    
    public Particle this[IntVec2 pos]
    {
        get
        {
            var index = pos.y * _size.x + pos.x;
            return _particles[index];
        }
        private set
        {
            var index = pos.y * _size.x + pos.x;
            _particles[index] = value;
        }
    }

    public ParticleGrid(int width, int height)
    {
        _size = new IntVec2(width, height);
        _chunkSize = new IntVec2(width/ChunkSize, height/ChunkSize);
        _particles = new Particle[width * height];
        _dataChunks = new ParticleChunk[_chunkSize.Area];

        for (int x = 0; x < _chunkSize.x; x++)
        {
            for (int y = 0; y < _chunkSize.y; y++)
            {
                var bounds = new IntRect(x * ChunkSize, y * ChunkSize, ChunkSize, ChunkSize);
                _dataChunks[y * _chunkSize.x + x] = new ParticleChunk(bounds);
            }
        }
    }

    private uint frame = 0;
    
    public void Step(float delta)
    {
        if (frame % 8 == 0)
        {
            Set(new IntVec2(130, 120), new Particle
            {
                id = MaterialType.Sand,
                lifeTime = 2,
                velocity = new Vector2(0, 1),
                color = MaterialDB.GetColor(MaterialType.Sand, 0),
                hasBeenUpdated = true
            }, true);
        }

        for (int i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            UpdateDirtyRect(ref chunk);
            _dataChunks[i] = chunk;
        }

        for (int i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            var dr = chunk.DirtyRect;
            for (int x = dr.x; x < dr.XMax; x++)
            {
                for (int y = dr.y; y < dr.YMax; y++)
                {
                    var index = y * _size.x + x;
                    var particle = _particles[index];
                    if (particle.id != MaterialType.Empty)
                    {
                        //Step Particle
                        PixelFunctions.Update(index, new IntVec2(x,y), particle, this);
                    }
                }
            }
        }
        
        frame++;
    }

    private void UpdateDirtyRect(ref ParticleChunk chunk)
    {
        var workingRect = new IntRect(0, 0, 0, 0);
        var minX = chunk.Bounds.XMax;
        var minY = chunk.Bounds.YMax;

        var maxX = chunk.Bounds.x;
        var maxY = chunk.Bounds.y;

        for (int y = chunk.Bounds.y; y < chunk.Bounds.YMax; y++)
        {
            for (int x = chunk.Bounds.x; x < chunk.Bounds.XMax; x++)
            {
                var index = y * _size.x + x;
                var particle = _particles[index];

                if (!particle.hasBeenUpdated)
                {
                    continue;
                }

                const int paddingHalf = 0;

                var checkX = x - paddingHalf;
                var checkY = y - paddingHalf;

                if (checkX < minX)
                {
                    minX = Math.Clamp(checkX, 0, _size.x - 1);
                    workingRect.x = minX;
                }

                if (checkY < minY)
                {
                    minY = Math.Clamp(checkY, 0, _size.y - 1);
                    workingRect.y = minY;
                }

                if (checkX > maxX)
                {
                    maxX = checkX;
                }

                if (checkY > maxY)
                {
                    maxY = checkY;
                }

                particle.hasBeenUpdated = false;
                _particles[index] = particle;
            }
        }

        workingRect.x = Math.Clamp(workingRect.x - 1, 0, _size.x - 1);
        workingRect.y = Math.Clamp(workingRect.y - 1, 0, _size.x - 1);
        workingRect.width = Math.Clamp((maxX - minX + 1) + 2, 0, (_size.x - workingRect.x) - 1);
        workingRect.height = Math.Clamp((maxY - minY + 1) + 2, 0, (_size.y - workingRect.y) - 1);

        chunk.SetDirty(workingRect);
    }

    public void HandleInput(TeleEventArgs args)
    {
        return;
        var x = args.MouseX;
        var y = args.MouseY;
        if (args.MouseButton == Mouse.Button.Left)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    Set(new IntVec2(x + i, y + k), new Particle
                    {
                        id = MaterialType.Sand,
                        lifeTime = 2,
                        velocity = new Vector2(0, 1),
                        color = MaterialDB.GetColor(MaterialType.Sand, 0),
                        hasBeenUpdated = true
                    }, true);
                }
            }
        }
    }
    
    public void Draw()
    {
        for (var i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            Widgets.Rectangle(
                new Rect(chunk.DirtyRect.x, chunk.DirtyRect.y, chunk.DirtyRect.width, chunk.DirtyRect.height),
                Color.Red, Color.Transparent);
        }
    }

    public void Set(IntVec2 pos, Particle p, bool updated = false)
    {
        p.hasBeenUpdated = updated;
        FS_Current.World.SetPixel(pos.x,pos.y, p.color);
        this[pos] = p;
    }
}
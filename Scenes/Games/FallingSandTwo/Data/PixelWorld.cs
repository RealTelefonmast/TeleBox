using SFML.Graphics;
using TeleBox.Engine.Data.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.UI;
using IntRect = TeleBox.Engine.Data.Primitive.IntRect;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

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
            for (int i = 0; i < 8; i++)
            {
                Set(new IntVec2(PixelWorldConfig.ChunkSize / 2 + i, PixelWorldConfig.ChunkSize / 2), new Particle
                {
                    id = MaterialType.Sand,
                    lifeTime = 2,
                    velocity = new Vector2(0, 1),
                    color = MaterialDB.GetColor(MaterialType.Sand, 0),
                    hasBeenUpdated = true
                }, true);
            }
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

        workingRect.width = maxX - minX + 1;
        workingRect.height = maxY - minY + 1;

        chunk.SetDirty(workingRect);
    }

    public void Draw()
    {
        for (var i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            Widgets.Rectangle(new Rect(chunk.DirtyRect.x,chunk.DirtyRect.y, chunk.DirtyRect.width, chunk.DirtyRect.height) , Color.Red, Color.Transparent);
            Widgets.Label($"[{i}]" + chunk.Bounds, new Rect(chunk.Bounds.x, chunk.Bounds.y, chunk.Bounds.width, 12));
        }
    }

    public void Set(IntVec2 pos, Particle p, bool updated = false)
    {
        p.hasBeenUpdated = updated;
        FS_Current.World.SetPixel(pos.x,pos.y, p.color);
        this[pos] = p;
    }
}

public class PixelWorld : Drawable
{
    private readonly ParticleGrid _grid;
    
    //Graphics
    private readonly RWTexture _texture;
    private readonly Sprite _sprite;
    private readonly Shader _shader;
    
    //private readonly PixelChunk[] _chunks;

    public PixelWorld(int width, int height)
    {
        FS_Current.World = this;

        _grid = new ParticleGrid(width, height);
        
        //Graphics
        _texture = new RWTexture(width, height);
        _sprite = new Sprite();
        _sprite.Texture = _texture.Texture;
        var fragmentShaderFile = Properties.Resources.simpleShader;
        _shader = new Shader(null, null, new MemoryStream(fragmentShaderFile));
        _shader.SetUniform("texture", Shader.CurrentTexture);
        
        //Game
        /*_chunks = new PixelChunk[width * height / (PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize)];
        
        for (int i = 0; i < _chunks.Length; i++)
        {
            var y = ((i * PixelWorldConfig.ChunkSize) / width) * PixelWorldConfig.ChunkSize;
            var x =((i * PixelWorldConfig.ChunkSize) % width);
            var widthChunk = PixelWorldConfig.ChunkSize;  // GridUtils.Constrained(PixelWorldConfig.ChunkSize, (uint)width, x);
            var heightChunk = PixelWorldConfig.ChunkSize; // GridUtils.Constrained(PixelWorldConfig.ChunkSize, (uint)height, y);
            _chunks[i] = new PixelChunk(i, new PixelRect
            {
                x = x,
                y = y,
                width = widthChunk,
                height = heightChunk,
            });
        }

        for (int i = 0; i < _chunks.Length; i++)
        {
            var chunk = _chunks[i];
            var pos = IntVec2.FromIndex(i, 16);// GridUtils.Position(i, );
            for (int k = 0; k < 4; k++)
            {
                var adj = pos + GenAdj.AdjacentCells4Way[k];
                if (adj.InBounds(new IntVec2(16, 8)))
                {
                    var index = adj.ToIndex(new IntVec2(16,8));
                    chunk.SetNeighbor(_chunks[index], k);
                }
            }
            
        }*/
    }

    private int counter = 0;

    public void Update(float delta)
    {
        _grid.Step(delta);
        /*if (counter == 4)
        {
            // for (int i = 0; i < 8; i++)
            // {
            //     _chunks[1].Set((64 + i),  (64), new Particle
            //     {
            //         id = MaterialType.Sand,
            //         lifeTime = 2,
            //         velocity = new Vector2(0, 1),
            //         color = MaterialDB.GetColor(MaterialType.Sand, 0),
            //         hasBeenUpdated = true
            //     });
            // }

           for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    _chunks[1].Set( (128 + i),  (128 + j), new Particle
                    {
                        id = MaterialType.Water,
                        lifeTime = 2,
                        velocity = new Vector2(0, 1),
                        color = MaterialDB.GetColor(MaterialType.Water, 0),
                        hasBeenUpdated = true
                    });
                }
            }
            
            counter = 0;
        }
        counter++;

        foreach (var chunk in _chunks)
        {
            chunk.Update(delta);
        }*/
        _texture.Update();
    }

    //
    public void Draw(RenderTarget target, RenderStates states)
    {
        if (Shader.IsAvailable)
        {
            states = new RenderStates(states)
            {
                Shader = _shader
            };
            UIRoot.Window.Draw(_sprite, states);
            
            // foreach (var chunk in _chunks)
            // {
            //     Widgets.Label( $"[{chunk.ID}]" + chunk.Bounds.ToString(), new Rect(chunk.Bounds.x, chunk.Bounds.y, chunk.Bounds.width, 12));
            // }
            //
            // foreach (var chunk in _chunks)
            // {
            //     chunk.DrawDebug();
            // }
        }
        
        //
        _grid.Draw();
    }

    public void SetPixel(int x, int y, Color particleColor)
    {
        _texture.SetPixel(x,y, particleColor);
    }
}
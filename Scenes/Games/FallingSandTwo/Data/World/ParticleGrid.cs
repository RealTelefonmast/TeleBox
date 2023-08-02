using SFML.Graphics;
using SFML.Window;
using TeleBox.Engine.Data;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;
using IntRect = TeleBox.Engine.Data.Primitive.IntRect;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data.World;


public unsafe struct ParticleGridBlock
{
    private fixed byte _grid[1024 * 512 * 11];
    private fixed byte _buffer[1024 * 512 * 11];
    
    public Particle this[int index]
    {
        get
        {
            fixed (byte* p = _grid)
            {
                return *(Particle*)(p + index * sizeof(Particle));
            }
        }
        set
        {
            fixed (byte* p = _grid)
            {
                *(Particle*)(p + index * sizeof(Particle)) = value;
            }
        }
    }
}

public unsafe class ParticleGrid
{
    //Main Buffer
    private IntVec2 _gridSize;
    private IntVec2 _chunkGridSize;
    //private fixed byte _particles[1024 * 512 * 11];
    private readonly Particle[] _particles;
    private readonly Particle[] _buffer;
    private readonly ParticleChunk[] _dataChunks;

    private static int ChunkSize => PixelWorldConfig.ChunkSize;
    public IntVec2 Size => _gridSize;

    
    public Particle this[int index] => _particles[index];
    
    public Particle this[IntVec2 pos]
    {
        get
        {
            var index = pos.y * _gridSize.x + pos.x;
            return _particles[index];
        }
        private set
        {
            var index = pos.y * _gridSize.x + pos.x;
            _particles[index] = value;
        }
    }

    public ParticleGrid(int width, int height)
    {
        _gridSize = new IntVec2(width, height);
        _chunkGridSize = new IntVec2(width/ChunkSize, height/ChunkSize);
        _particles = new Particle[width * height];
        _buffer = new Particle[width * height];
        _dataChunks = new ParticleChunk[_chunkGridSize.Area];

        for (int x = 0; x < _chunkGridSize.x; x++)
        {
            for (int y = 0; y < _chunkGridSize.y; y++)
            {
                var bounds = new IntRect(x * ChunkSize, y * ChunkSize, ChunkSize, ChunkSize);
                _dataChunks[y * _chunkGridSize.x + x] = new ParticleChunk(bounds);
            }
        }
    }

    private uint frame = 0;

    public void Step(float delta)
    {
        for (int i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            UpdateDirtyRect(ref chunk);
            _dataChunks[i] = chunk;
        }

        //Array.Copy(_particles, this._buffer, this._particles.Length);


        for (int c = 0; c < _dataChunks.Length; c++)
        {
            var chunk = _dataChunks[c];
            var dr = chunk.DirtyRect;
            fixed (int* indices = new int[dr.Area])
            {
                int bigInd = 0;
                for (int y = dr.y; y < dr.YMax; y++)
                {
                    for (int x = dr.x; x < dr.XMax; x++)
                    {
                        var index = y * _gridSize.x + x;
                        indices[bigInd] = index;
                        bigInd++;
                    }
                }

                FisherYatesShuffle(indices, dr.Area);

                for (int i = 0; i < dr.Area; i++)
                {
                    var index = indices[i];
                    var particle = _particles[index];
                    if (particle.id != MaterialType.Empty)
                    {
                        //Step Particle
                        PixelFunctions.Update(index, new IntVec2(index % _gridSize.x, index / _gridSize.x),particle, this);
                    }
                }
            }

            frame++;
        }
    }

    private static unsafe void FisherYatesShuffle<T>(T* array, int length) where T : unmanaged
    {
        for (var i = length - 1; i > 0; i--)
        {
            var j = Rand.Range(0, i+1);
            (array[i], array[j]) = (array[j], array[i]);
        }
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
                var index = y * _gridSize.x + x;
                var particle = _particles[index];

                if (!particle.hasBeenUpdated)
                {
                    continue;
                }

                const int padding = 0;

                var checkX = x - padding;
                var checkY = y - padding;

                if (checkX < minX)
                {
                    minX = Math.Clamp(checkX, 0, _gridSize.x - 1);
                    workingRect.x = minX;
                }

                if (checkY < minY)
                {
                    minY = Math.Clamp(checkY, 0, _gridSize.y - 1);
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

        if (workingRect.x > 0)
            _ = " v";

        workingRect.x = Math.Clamp(workingRect.x - 2, 0, _gridSize.x - 1);
        workingRect.y = Math.Clamp(workingRect.y - 2, 0, _gridSize.y - 1);
        workingRect.width = Math.Clamp((maxX - minX) + 4, 0, _gridSize.x - workingRect.x);
        workingRect.height = Math.Clamp((maxY - minY) + 4, 0, _gridSize.y - workingRect.y);
        chunk.SetDirty(workingRect);
    }

    public void HandleInput(TEvent args)
    {
        var pos = args.MousePos;
        if (PixelFunctions.Inbounds(pos, this))
        {
            var leftDown = TEvent.IsMouseDown(Mouse.Button.Left);
            var rightDown = TEvent.IsMouseDown(Mouse.Button.Right);
            if (leftDown)
            {
                for (int i = 0; i < 24; i++)
                {
                    for (int k = 0; k < 24; k++)
                    {
                        Set(new IntVec2(pos.x + i, pos.y + k),
                            new Particle( MaterialType.Stone )
                            {
                                lifeTime = 2,
                                velocity = new Vector2(0, 1),
                                hasBeenUpdated = true
                            }, true);
                    }
                }
            }
            if (rightDown)
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int k = 0; k < 32; k++)
                    {
                        Set(new IntVec2(pos.x + i, pos.y + k),
                            new Particle(MaterialType.Water)
                            {
                                lifeTime = 2,
                                velocity = new Vector2(0, 1),
                                hasBeenUpdated = true
                            }, true);
                    }
                }
            }
        }
    }
    
    public void Draw()
    {
        for (var i = 0; i < _dataChunks.Length; i++)
        {
            var chunk = _dataChunks[i];
            Widgets.Rectangle(chunk.Bounds.ToRect(),new Color(0,255,0,125), Color.Transparent);
            Widgets.Rectangle(chunk.DirtyRect.ToRect(),new Color(255,0,0,150), Color.Transparent);
        }
    }

    public void Set(IntVec2 pos, Particle p, bool updated = false)
    {
        p.hasBeenUpdated = updated;
        FS_Current.World.SetPixel(pos.x,pos.y, p.color);
        this[pos] = p;
    }
}
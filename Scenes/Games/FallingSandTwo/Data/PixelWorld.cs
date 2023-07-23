using SFML.Graphics;
using TeleBox.Engine.Data.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public class ParticleChunk
{
    private Rect _bounds;
    private Rect _dirtyRect;
}

public class PixelWorld : Drawable
{
    //Main Buffer
    private readonly Particle[] _particles;
    private readonly ParticleChunk[] _dataChunks;
    
    //Graphics
    private readonly RWTexture _texture;
    private readonly Sprite _sprite;
    private readonly Shader _shader;
    
    private readonly PixelChunk[] _chunks;

    public PixelWorld(int width, int height)
    {
        FS_Current.World = this;

        _particles = new Particle[width * height];
        _dataChunks = new ParticleChunk[width * height / (PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize)];
        
        //Graphics
        _texture = new RWTexture(width, height);
        _sprite = new Sprite();
        _sprite.Texture = _texture.Texture;
        var fragmentShaderFile = Properties.Resources.simpleShader;
        _shader = new Shader(null, null, new MemoryStream(fragmentShaderFile));
        _shader.SetUniform("texture", Shader.CurrentTexture);
        
        //Game
        _chunks = new PixelChunk[width * height / (PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize)];
        
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
            var pos = IntVec2.FromIndex(i, 8);// GridUtils.Position(i, );
            for (int k = 0; k < 4; k++)
            {
                var adj = pos + GenAdj.AdjacentCells4Way[k];
                if (adj.InBounds(new IntVec2(8, 4)))
                {
                    var index = adj.ToIndex(new IntVec2(8,4));
                    chunk.SetNeighbor(_chunks[index], k);
                }
            }
            
        }
    }

    private int counter = 0;

    public void Update(float delta)
    {
        if (counter == 4)
        {
            for (int i = 0; i < 8; i++)
            {
                _chunks[1].Set((64 + i),  (64), new Particle
                {
                    id = MaterialType.Sand,
                    lifeTime = 2,
                    velocity = new Vector2(0, 1),
                    color = MaterialDB.GetColor(MaterialType.Sand, 0),
                    hasBeenUpdated = true
                });
            }

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
        }
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
            foreach (var chunk in _chunks)
            {
                Widgets.Label( $"[{chunk.ID}]" + chunk.Bounds.ToString(), new Rect(chunk.Bounds.x, chunk.Bounds.y, chunk.Bounds.width, 12));
            }

            foreach (var chunk in _chunks)
            {
                chunk.DrawDebug();
            }
        }
    }

    public void SetPixel(int x, int y, Color particleColor)
    {
        _texture.SetPixel(x,y, particleColor);
    }
}
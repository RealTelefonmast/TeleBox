using SFML.Graphics;
using TeleBox.Engine.Data.Graphics;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public class PixelWorld : Drawable
{
    //Graphics
    private RWTexture _texture;
    private Sprite _sprite;
    private Shader _shader;
    
    private PixelChunk[] _chunks;

    public PixelWorld(int width, int height)
    {
        FS_Current.World = this;
        
        //Graphics
        _texture = new RWTexture(width, height);
        _sprite = new Sprite();
        _sprite.Texture = _texture.Texture;
        var fragmentShaderFile = Properties.Resources.simpleShader;
        _shader = new Shader(null, null, new MemoryStream(fragmentShaderFile));
        _shader.SetUniform("texture", Shader.CurrentTexture);
        
        //Game
        _chunks = new PixelChunk[width * height / (PixelWorldConfig.ChunkSize * PixelWorldConfig.ChunkSize)];
        for (uint i = 0; i < _chunks.Length; i++)
        {
            var y = (uint)((i * PixelWorldConfig.ChunkSize) / width) * PixelWorldConfig.ChunkSize;
            var x = (uint)((i * PixelWorldConfig.ChunkSize) % width);
            var widthChunk = PixelWorldConfig.ChunkSize;  // GridUtils.Constrained(PixelWorldConfig.ChunkSize, (uint)width, x);
            var heightChunk = PixelWorldConfig.ChunkSize; // GridUtils.Constrained(PixelWorldConfig.ChunkSize, (uint)height, y);
            _chunks[i] = new PixelChunk(new PixelRect
            {
                x = x,
                y = y,
                width = widthChunk,
                height = heightChunk,
            });
        }
    }

    private int counter = 0;

    public void Update(float delta)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++ )
            {
                _chunks[1].Set((uint)(64 + i), (uint)(64 + j), new Particle
                {
                    id = MaterialType.Sand,
                    lifeTime = 2,
                    velocity = new Vector2(0, 1),
                    color = Color.Yellow,
                    hasBeenUpdated = false
                });
                
                _chunks[1].Set((uint)(128 + i), (uint)(128 + j), new Particle
                {
                    id = MaterialType.Sand,
                    lifeTime = 2,
                    velocity = new Vector2(0, 1),
                    color = Color.Yellow,
                    hasBeenUpdated = false
                });
            }
        }
        
        _chunks[1].Update(delta);

        // for (uint i = 0; i < _chunks.Length; i++)
        // {
        //     _chunks[i].Update(delta);
        // }

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
                Widgets.Label(chunk.Bounds.ToString(), new Rect(chunk.Bounds.x, chunk.Bounds.y, chunk.Bounds.width, 12));
                //Widgets.Rectangle(chunk.Bounds, Color.Red, Color.Transparent);
            }
            //target.Draw(_sprite, states);
        }
    }

    public void SetPixel(int x, int y, Color particleColor)
    {
        _texture.SetPixel(x,y, particleColor);
    }
}
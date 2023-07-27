using SFML.Graphics;
using TeleBox.Engine.Data.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Games.FallingSandTwo.Data.World;
using TeleBox.UI;
using IntRect = TeleBox.Engine.Data.Primitive.IntRect;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

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
    }

    private int counter = 0;

    public void Update(float delta)
    {
        _grid.Step(delta);
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

    public void HandleEvents(TEvent args)
    {
        _grid.HandleInput(args);
    }
}
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.Window;
using TeleBox.Engine.Data;
using TeleBox.Engine.Data.Graphics;
using TeleBox.Engine.Data.Physics;
using TeleBox.Engine.Data.Physics.Data;
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
        
        //
        physics = new Physics();
    }

    private int counter = 0;

    private Physics physics;
    
    public void Update(float delta)
    {
        _grid.Step(delta);
        _texture.Update();
        physics.UpdateVerlet();
    }

    //
    public void Draw(RenderTarget target, RenderStates states)
    {
        physics.Render(new RenderArgs
        {
            Target = target,
            States = states,
        });
        
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

    public unsafe void HandleEvents(TEvent args)
    {
        _grid.HandleInput(args);
     
        var pos = args.MousePos;
        if (PixelFunctions.Inbounds(pos, _grid))
        {
            if (TEvent.IsMouseDown(Mouse.Button.Left))
            {
                PixelShape poly = new PixelShape();
                int count = Rand.Range( 3, 32 );
                fixed (Vector2* vertices = new Vector2[count])
                {
                    float e = Rand.Range(5, 10);
                    for (int i = 0; i < count; ++i)
                        vertices[i] = new Vector2(Rand.Range(-e, e), Rand.Range(-e, e));
                    poly.Set(vertices, count);
                    RigidBody* b = physics.Add(&poly, pos.x, pos.y);
                    b->SetOrient( Rand.Range( -Const.PI, Const.PI ) );
                    b->restitution = 0.2f;
                    b->dynamicFriction = 0.2f;
                    b->staticFriction = 0.4f;
                }
            }
        }
    }
}
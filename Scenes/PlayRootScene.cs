using SFML.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;

namespace TeleBox.Scenes;

public class PlayRootScene : Scene
{
    public override string Name => "PlayScene";

    private int frames = 0;
    private Color color = Color.White;
    
    
    public override void Update(float delta)
    {
    }

    public override void Draw(RenderArgs args)
    {
        frames++;
        if (frames == 120)
        {
            frames = 0;
        }
        
        Widgets.Rectangle(new Rect(frames,frames, UIRoot.Width-frames*2, UIRoot.Height-frames*2), Color.White, color);

    }
}
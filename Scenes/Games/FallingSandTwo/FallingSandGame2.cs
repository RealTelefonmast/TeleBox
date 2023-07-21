using TeleBox.Scenes.Games.FallingSandTwo.Data;
using TeleBox.UI;

namespace TeleBox.Scenes.FallingSandTwo;

public class FallingSandGame2 : BaseGame
{
    public PixelWorld _world;
    
    public override string Name => "FS2";

    public FallingSandGame2()
    {
        _world = new PixelWorld(UIRoot.Width, UIRoot.Height);
    }
    
    public override void Update(float delta)
    {
        _world.Update(delta);
    }

    public override void Draw(RenderArgs args)
    {
        args.Target.Draw(_world);
    }
}
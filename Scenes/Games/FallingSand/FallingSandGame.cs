using SFML.Graphics;
using SFML.Window;
using TeleBox.Scenes.Games.FallingSand;
using TeleBox.Scenes.Materials;
using TeleBox.UI;

namespace TeleBox.Scenes;

public class FallingSandGame : BaseGame
{
    private World _world;
    private static float _delta;
    private static long _ticks;
    private static bool _isSimulating;
    
    private const int _controlButtonWidth = 70;
    private const int _controlButtonHeight = 30;
    private const int _buttonWidth = 70;
    private const int _buttonHeight = 30;
    
    public override string Name => "Falling_Sand";

    public FallingSandGame()
    {
        _world = new World(UIRoot.Width, UIRoot.Height);
        _world.SetMaterial(MaterialType.Sand);
        _world.SetRadius(10);
        _isSimulating = true;
    }

    public override void Update(float delta)
    {
        _ticks += 1;
        if (_ticks == long.MaxValue)
        {
            _ticks = 0;
        }
        
        _world.Update();
    }

    public override void HandleEvents(TeleEventArgs args)
    {
        base.HandleEvents(args);
        _world.StartInput(args.MouseButton, args.MouseX, args.MouseY);
        _world.Input();
        _world.StopInput();
    }

    private void RenderUI(RenderArgs args)
    {
        
    }

    private void RenderGame(RenderArgs args)
    {
        args.Target.Draw(_world);
    }
    
    public override void Draw(RenderArgs args)
    {
        RenderGame(args);
        RenderUI(args);
    }
}
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;

namespace TeleBox.Scenes;

public class MainMenu : Scene
{
    public override string Name => "MainMenu";
    
    public override void Update(float delta)
    {
        
    }
    
    private bool shouldDrawSelection = false;
    
    public override void Draw(RenderArgs args)
    {
        var topBar = args.Rect.TopPart(30);
        var buttonRect = topBar.ContractedBy(3);
        var buttonStart = buttonRect.LeftPart(100);
        var buttonQuit = buttonRect.RightPart(100);
        var selectionRect = new Rect(0, 35, 150, 400);

        Widgets.Label("Tele-Box", new Rect(0, args.Rect.height - 30, 100, 30));
        
        var mousePos = Mouse.GetPosition();
        var mousePos2 = UIRoot.MousePosition;
        Widgets.Label($"({mousePos.X}, {mousePos.Y})", new Rect(UIRoot.Width - 100, UIRoot.Height - 100, 100, 12));
        Widgets.Label($"({mousePos2.X}, {mousePos2.Y})", new Rect(UIRoot.Width - 100, UIRoot.Height - 84, 100, 12));
        
        Widgets.Rectangle(topBar, Color.Black, new Color(255,255,255,125));
        
        if (Widgets.Button("Play", buttonStart))
        {
            shouldDrawSelection = !shouldDrawSelection;
        }
        
        if (Widgets.Button("Quit", buttonQuit))
        {
            var num = 0;
        }

        if (shouldDrawSelection)
        {
            DrawSelection(selectionRect);
        }
    }

    private void DrawSelection(Rect rect)
    {
        Widgets.Rectangle(rect, Color.White, new Color(0, 255, 255, 125));

        for (var i = 0; i < SceneManager.Instance.Games.Count; i++)
        {
            var game = SceneManager.Instance.Games[i];
            var selRect = new Rect(rect.x + 5, (rect.y + 5) + i * 24, rect.width - 10, 24);
            if (Widgets.Button(game.Name, selRect))
            {
                UIRoot.SwitchScene(game.Name);
            }
        }
    }
}
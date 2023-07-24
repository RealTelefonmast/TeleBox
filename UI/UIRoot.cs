using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TeleBox.Engine;
using TeleBox.Engine.Data.Primitive;
using Color = SFML.Graphics.Color;

namespace TeleBox.UI;

internal static class UIRoot
{
    internal static Font MainFont = new Font(Properties.Resources.UniSansThin);
    internal static Shader MainShader = new Shader(null, null, new MemoryStream(Properties.Resources.simpleShader));
    
    //UI
    private static RenderWindow _window;
    private static SceneManager _sceneManager;
    
    //
    private static float _delta;
    private static long _ticks;
    public static Rect UIRect => new(0, 0, Width, Height);

    public static RenderWindow Window => _window;
    public static SceneManager SceneManager => _sceneManager;
    
    public static Vector2f MousePosition { get; private set; }
    public static int Width => (int)_window.Size.X;
    public static int Height => (int)_window.Size.Y;
    public static bool IsReady { get; set; }

    private static TeleEventArgs CurrentEvents;
    
    internal static void Init(VideoMode mode, string title, Styles style)
    {
        _window = new RenderWindow(mode, title, style);
        _window.SetVerticalSyncEnabled(true);
        _window.Closed += (_, _) => _window.Close();
        _window.MouseButtonPressed += OnMouseButtonPressed;
        _window.MouseButtonReleased += OnMouseButtonReleased;
        _window.MouseMoved += OnMouseMoved;
        _window.MouseWheelScrolled += OnMouseWheelScrolled;
        _window.KeyPressed += OnKeyPressed;
        _sceneManager = new SceneManager(_window);
        IsReady = true;
    }

    internal static void Start()
    {
        var colorTop = new Color(10, 10, 20);
        var colorBottom = new Color(30, 20, 10);
        var gradient = new Vertex[4];
        gradient[0] = new Vertex(new Vector2f(0, 0), colorTop);
        gradient[1] = new Vertex(new Vector2f(0, Height), colorBottom);
        gradient[2] = new Vertex(new Vector2f(Width, Height), colorBottom);
        gradient[3] = new Vertex(new Vector2f(Width, 0), colorTop);
        
        while (Window.IsOpen)
        {
            _ticks++;
            if (_ticks == long.MaxValue) 
                _ticks = 0;
            
            Window.DispatchEvents();
            
            BeginFrame();
            {
                _sceneManager.HandleEvents(CurrentEvents);
                //SceneManager.Update(_delta);
            }
            EndFrame();
            
            Window.Clear(Color.Black);
            Window.Draw(gradient, PrimitiveType.Quads);
            _sceneManager.Render();
            Window.Display();

            if (_ticks % 20 == 0)
            {
                if (PlayRoot.Ready)
                {
                    var milliseconds = (Math.Round(_delta * 1000, 2) + "").Replace(',', '.');
                    var title =
                        $"{Program.AppName} ({milliseconds} ms per frame) | {PlayRoot.Watch.ElapsedMilliseconds} ms per frame calc";
                    Window.SetTitle(title);
                }
            }
        }
    }

    private static void BeginFrame()
    {
        Window.DispatchEvents();
        Window.Clear(Color.Black);
        CurrentEvents = new TeleEventArgs
        {
            MouseButton = Event.CurMouseState._button,
            MouseX = (int)Event.CurMouseState._x,
            MouseY = (int)Event.CurMouseState._y,

        };
    }

    private static void EndFrame()
    {
        CurrentEvents = TeleEventArgs.Empty;
    }

    public static void Draw(Drawable drawable)
    {
        _window.Draw(drawable);
    }

    #region Scenes

    public static void SwitchScene(string tag)
    {
        _sceneManager.Switch(tag);
    }
    
    #endregion
    
    #region EventHandling
    
    private static void OnKeyPressed(object? sender, KeyEventArgs e)
    {
    }

    private static void OnMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
    {
    }

    private static void OnMouseMoved(object? sender, MouseMoveEventArgs e)
    {
        MousePosition = new Vector2f(e.X, e.Y);
    }

    private static void OnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
    {
        Event.Notify_MouseEvent(e.Button, MouseEvent.Released);
    }

    private static void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        Event.Notify_MouseEvent(e.Button, MouseEvent.Pressed);
    }
    
    #endregion
    
}
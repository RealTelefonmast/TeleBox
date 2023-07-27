using System.Drawing;
using SFML.System;
using SFML.Window;
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.UI;

[Flags]
public enum MouseEventType
{
    Moved,
    Scrolled,
    Pressed,
    Released,
}

public struct MouseEvent
{
    public Mouse.Button Button { get; set; }
    public MouseEventType Type { get; set; }

    public static MouseEvent Invalid => new MouseEvent
    {
        Button = (Mouse.Button)99,
        Type = (MouseEventType)99,
    };

    public void SetMoved()
    {
        Type |= MouseEventType.Moved;
    }
}

//Event Pipeline

// - Input happens, record it
// Keep track of mouse location and inputs

//Frame Begins
//Set current mouse pos and events as nullable struct

//Frame Ends
//Set everything back to null

public struct TEvent
{
    //Constantly Tracked
    private static MouseEvent _prevMouse;
    private static MouseEvent _curMouse;
    
    private static int _x { get; set; }
    private static int _y { get; set; }

    public IntVec2 MousePos => new IntVec2(_x, _y);
    public Vector2 MousePosF => new Vector2(_x, _y);
    
    //Current Frame
    private MouseEvent? MouseP { get; set; }
    public MouseEvent Mouse => MouseP ?? MouseEvent.Invalid;
    
    public static TEvent Current { get; private set; }

    public static void BeginFrame()
    {
        Current = new TEvent()
        {
            MouseP = _curMouse
        };
    }
    
    public static void Consume()
    {
        Current = new TEvent() {MouseP = MouseEvent.Invalid};
    }

    #region Mouse

    public static bool Clicked(Mouse.Button button)
    {
        return _prevMouse.Button == button && _prevMouse.Type == MouseEventType.Pressed && 
               _curMouse.Button == button && _curMouse.Type == MouseEventType.Released;
    }

    public static void Set_MouseEvent(MouseMoveEventArgs e, MouseEventType moved)
    {
        _prevMouse = _curMouse;
        _curMouse.SetMoved();
        _x = e.X;
        _y = e.Y;
    }

    public static void Set_MouseEvent(MouseButtonEventArgs e, MouseEventType type)
    {
        _prevMouse = _curMouse;
        _curMouse.Button = e.Button;
        _curMouse.Type = type;
    }
    
    #endregion

    public static bool IsMouseDown(Mouse.Button button)
    {
        return Current.Mouse.Button == button && Current.Mouse.Type == MouseEventType.Pressed;
    }
}
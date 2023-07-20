using System.Drawing;
using System.Numerics;
using SFML.System;
using SFML.Window;

namespace TeleBox.UI;

public class EventData
{
    public Mouse.Button MouseUp { get; set; }
    public bool MouseMove { get; set; }
    public bool MouseWheel { get; set; }
    
    public Keyboard.Key KeyDown { get; set; }

    public void Clear()
    {
    }
}

public enum MouseEvent
{
    Pressed,
    Released,
}

public struct MouseState
{
    public Mouse.Button _button;
    public MouseEvent _event;
    public float _x;
    public float _y;
}

public static class Event
{
    private static MouseState _prevMouseState;
    private static MouseState _currMouseState;

    public static Vector2f MousePosition => UIRoot.MousePosition;

    public static MouseState CurMouseState => _currMouseState;
    
    public static bool IsMouseDown(Mouse.Button button)
    {
        return Mouse.IsButtonPressed(button);
    }

    public static void Notify_MouseEvent(Mouse.Button button, MouseEvent @event)
    {
        _prevMouseState = _currMouseState;
        _currMouseState._x = MousePosition.X;
        _currMouseState._y = MousePosition.Y;
        _currMouseState._button = button;
        _currMouseState._event = @event;
        
    }
    
    public static bool Clicked(Mouse.Button button)
    {
        return _prevMouseState._button == button && _prevMouseState._event == MouseEvent.Pressed 
                                                 && _currMouseState._button == button && _currMouseState._event == MouseEvent.Released;
    }
}
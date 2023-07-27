using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using SFML.Window;
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.UI;

public static class Widgets
{
    //TODO: Font overrides
    public static void Label(string text, Rect rect)
    {
        Label(text, rect, Color.White);
    }

    public static void Label(string text, Rect rect, Color color)
    {
        var textObj = new Text(text, UIRoot.MainFont, (uint)rect.height);
        textObj.Position = rect.Position;
        textObj.FillColor = color;
        UIRoot.Window.Draw(textObj);
    }

    internal static bool _isPointerDown = false;
    
    public static bool Button(string text, Rect rect)
    {
        var isHovering = rect.Contains(TEvent.Current.MousePos);
        bool isDownThisFrame = TEvent.IsMouseDown(Mouse.Button.Left);
        var bColor = isHovering ? Color.White : Color.Black;
        var fillColor = isHovering ? Color.Black : Color.White;

        if (isHovering)
        {
            if (isDownThisFrame)
            {
                // Mouse button is currently being pressed on the button
                fillColor = Color.Cyan;
                if (!_isPointerDown)
                {
                    // Mouse button was just pressed down on the button
                    _isPointerDown = true;
                }
            }
            else if (_isPointerDown)
            {
                // Mouse button was just released (either on the button or elsewhere)
                _isPointerDown = false;
                if (isHovering)
                {
                    // Mouse button was released on the button -> it was a complete click
                    Rectangle(rect, bColor, fillColor);
                    Label(text, rect, bColor);
                    return true;
                }
            }
        }

        // Below line should be executed when no click occurs or before the return statement
        Rectangle(rect, bColor, fillColor);
        Label(text, rect, bColor);
        return false;
    }

    public static void Rectangle(Rect rect, Color borderColor, Color fillColor)
    {
        var rectObj = new RectangleShape(rect.Size);
        rectObj.Position = rect.Position;
        rectObj.OutlineColor = borderColor;
        rectObj.FillColor = fillColor;
        rectObj.OutlineThickness = 1;
        UIRoot.Window.Draw(rectObj);
    }
}
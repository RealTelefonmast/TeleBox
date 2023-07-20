using System.Drawing;
using SFML.Graphics;
using SFML.System;

namespace TeleBox.UI;

public static class Extensions
{
    public static bool Contains(this IntRect rect, Vector2i point)
    {
        return rect.Contains(point.X, point.Y);
    }
    
    public static bool Contains(this IntRect rect, Point point)
    {
        return rect.Contains(point.X, point.Y);
    }
}
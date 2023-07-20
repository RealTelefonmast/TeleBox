﻿using SFML.Graphics;

namespace TeleBox.Scenes;

public sealed class MatrixUpdatedEventArgs : EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }
    public Color Color { get; set; }

    public MatrixUpdatedEventArgs(int x, int y, Color color)
    {
        X = x;
        Y = y;
        Color = color;
    }
}
﻿using SFML.Graphics;

namespace TeleBox.Engine.Data.Graphics;

public sealed class RWTexture
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Array { get; }
    public Texture Texture { get; }
    
    public RWTexture(int w, int h)
    {
        Width = w;
        Height = h;
        Array = new byte[Width * Height * 4];
        Texture = new Texture((uint)Width, (uint)Height);
    }

    public void SetPixel(int x, int y, Color color)
    {
        var index = (x * 4) + (y * Width * 4);
        Array[index] = color.R;
        Array[index + 1] = color.G;
        Array[index + 2] = color.B;
        Array[index + 3] = color.A;
    }

    public Color GetPixel(int x, int y)
    {
        var index = (x * 4) + (y * Width * 4);
        var r = Array[index];
        var g = Array[index + 1];
        var b = Array[index + 2];
        var a = Array[index + 3];

        return new Color(r, g, b, a);
    }

    private bool Contains(int x, int y)
    {
        return (x >= 0 && x < Width && y >= 0 && y < Height);
    }

    public void DrawPoint(int x, int y, Color color)
    {
        if (Contains(x, y))
        {
            SetPixel(x, y, color);
        }
    }

    public void Clear(Color color)
    {
        for (int i = 0; i < Width * Height * 4; i += 4)
        {
            Array[i] = color.R;
            Array[i + 1] = color.G;
            Array[i + 2] = color.B;
            Array[i + 3] = color.A;
        }
    }

    public void Update()
    {
        Texture.Update(Array);
    }
}
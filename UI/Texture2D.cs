using SFML.Graphics;

namespace TeleBox.UI;

public class Texture2D : Texture
{
    public Texture2D(uint width, uint height) : base(width, height)
    {
    }

    public Texture2D(string filename) : base(filename)
    {
    }

    public Texture2D(string filename, IntRect area) : base(filename, area)
    {
    }

    public Texture2D(Stream stream) : base(stream)
    {
    }

    public Texture2D(Stream stream, IntRect area) : base(stream, area)
    {
    }

    public Texture2D(Image image) : base(image)
    {
    }

    public Texture2D(Image image, IntRect area) : base(image, area)
    {
    }

    public Texture2D(byte[] bytes) : base(bytes)
    {
    }

    public Texture2D(Texture copy) : base(copy)
    {
    }
}
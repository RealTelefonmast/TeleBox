using SFML.Graphics;
using SFML.Window;

namespace TeleBox.UI;

public class RenderTexture2D : RenderTexture
{
    public RenderTexture2D(uint width, uint height) : base(width, height)
    {
    }

    public RenderTexture2D(uint width, uint height, bool depthBuffer) : base(width, height, depthBuffer)
    {
    }

    public RenderTexture2D(uint width, uint height, ContextSettings contextSettings) : base(width, height, contextSettings)
    {
    }
}
using SFML.Window;

namespace TeleBox.UI;

public struct TeleEventArgs
{
    public Mouse.Button MouseButton { get; set; }
    public int MouseX { get; set; }
    public int MouseY { get; set; }
    
    public static TeleEventArgs Empty { get; } = new TeleEventArgs();
}
using System.Numerics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TeleBox.UI;

namespace TeleBox;

internal static class Program
{

    internal static string AppName = "TeleBox";
    private const uint _width = 1024;
    private const uint _height = 512;

    //
    
    [STAThread]
    private static void Main(string[] args)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        UIRoot.Init(new VideoMode(_width, _height), AppName, Styles.Close);
    }
}
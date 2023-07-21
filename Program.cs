using System.Numerics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TeleBox.Engine;
using TeleBox.UI;

namespace TeleBox;

internal static class Program
{
    internal static string AppName = "TeleBox";
    private const uint _width = 1024;
    private const uint _height = 512;

    public static Thread UIThread;
    public static Thread PlayThread;

    [STAThread]
    private static void Main(string[] args)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        UIThread = new Thread(UIThreadStart);
        PlayThread = new Thread(PlayThreadStart);
        while (UIRoot.Window.IsOpen)
        {
            //..
        }
    }

    private static void UIThreadStart()
    {
        UIRoot.Init(new VideoMode(_width, _height), AppName, Styles.Close);
    }

    private static void PlayThreadStart()
    {
        PlayRoot.Init();
    }
}
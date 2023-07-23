using System.Diagnostics;
using TeleBox.UI;

namespace TeleBox.Engine;

public static class PlayRoot
{
    private static double targetFrameTime = 1000d / 60; // 1000 ms / 60 frames per second gives us target time per frame in milliseconds
    private static Stopwatch _watch;

    public static Stopwatch Watch => _watch;
    public static bool Ready { get; set; }

    public static void Init()
    {
        _watch = new Stopwatch();
        Ready = true;
        PlayLoop();
    }

    private static void PlayLoop()
    {
        while (true)
        {
            if ((!UIRoot.IsReady)) continue;
            _watch.Reset();
            _watch.Start();

            UIRoot.SceneManager.Update(_watch.ElapsedMilliseconds);
            
            _watch.Stop();
            
            long elapsedTime = _watch.ElapsedMilliseconds;
            if(elapsedTime < targetFrameTime) {
                Thread.Sleep((int)(targetFrameTime - elapsedTime));
            }
        }
    }
}
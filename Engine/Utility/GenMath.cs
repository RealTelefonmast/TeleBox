namespace TeleBox.Engine.Utility;

public static class GenMath
{
    public static int Clamp(int num, int min, int max)
    {
        return num < min ? min : num > max ? max : num;
    }
}
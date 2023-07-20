namespace TeleBox.Engine.Utility;

public static class Rand
{
    private static readonly Random _r = new Random();
    
    public static int Next(int min, int max)
    {
        return _r.Next(min, max);
    }
    
    public static int Range(int lower, int upper)
    {
        if (lower > upper)
        {
            (lower, upper) = (upper, lower);
        }

        return _r.Next(int.MaxValue) % (upper - lower + 1) + lower;
    }
    
    public static double NextDouble()
    {
        return _r.NextDouble();
    }
    
    public static bool NextBoolean()
    {
        return _r.Next() > (int.MaxValue / 2);
    }

}
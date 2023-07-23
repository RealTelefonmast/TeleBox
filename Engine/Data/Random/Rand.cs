namespace TeleBox.Engine.Data;

public class Rand
{
    private static uint _seed;
    private static uint _iterations = 0U;
    private static readonly Stack<ulong> _stateStack = new Stack<ulong>();
    
    public static int Seed
    {
        set
        {
            if (_stateStack.Count == 0)
                throw new Exception("Modifying the initial rand seed. Call PushState() first. The initial rand seed should always be based on the startup time and set only once.");
            _seed = (uint) value;
            _iterations = 0U;
        }
    }

    static Rand()
    {
        _seed = (uint) DateTime.Now.GetHashCode();
    }

    private static ulong StateCompressed
    {
        get => _seed | ((ulong) _iterations << 32);
        set
        {
            _seed = (uint)(value & 0xFFFFFFFF);
            _iterations = (uint)((value >> 32) & 0xFFFFFFFF);
        }
    }

    public static float Value => (float) ((MurmurHash.GetInt(_seed, _iterations++) - -2147483648.0) / 4294967295.0);
    public static bool Bool => Value < 0.5f;
    public static int Sign => !Bool ? -1 : 1;
    public static int Int => MurmurHash.GetInt(Rand._seed, Rand._iterations++);
    
    //
    public static bool Chance(float chance)
    {
        return chance > 0f && (chance >= 1f || Rand.Value < chance);
    }
    
    //
    public static void PushState()
    {
        _stateStack.Push(StateCompressed);
    }

    public static void PushState(int replacementSeed)
    {
        PushState();
        Seed = replacementSeed;
    }
    
    public static void PopState()
    {
        Rand.StateCompressed = Rand._stateStack.Pop();
    }

    public static int Range(int min, int max)
    {
        if (max <= min) return min;
        return min + Math.Abs(Int % (max - min));
    }

    public static int RangeInclusive(int min, int max)
    {
        if (max <= min) return min;
        return Range(min, max + 1);
    }

    public static float Range(float min, float max)
    {
        if (max <= min) return min;
        return Value * (max - min) + min;
    }

    
}
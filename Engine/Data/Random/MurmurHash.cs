namespace TeleBox.Engine.Data;

public static class MurmurHash
{
    public static int GetInt(uint seed, uint input)
    {
        const uint Const1 = 3432918353U;
        const uint Const2 = 461845907U;
        const uint Const3 = 3864292196U;
        const uint Const4Mix = 2246822507U;
        const uint Const5Mix = 3266489909U;
        const uint Const6StreamPosition = 2834544218U;

        var num = input * Const1;
        num = (num << 15) | (num >> 17);
        num *= Const2;
        var num2 = seed ^ num;
        num2 = (num2 << 13) | (num2 >> 19);
        num2 = num2 * 5U + Const3;
        num2 ^= Const6StreamPosition;
        num2 ^= num2 >> 16;
        num2 *= Const4Mix;
        num2 ^= num2 >> 13;
        num2 *= Const5Mix;
        return (int) (num2 ^ (num2 >> 16));
    }
}
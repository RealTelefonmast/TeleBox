using SFML.Graphics;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public sealed class ColorSamples
{
    private readonly List<Color> _colors;

    public ColorSamples()
    {
        _colors = new List<Color>();
    }

    public int Count => _colors.Count;

    public Color this[int index]
    {
        get => _colors[index];
        private set => _colors[index] = value;
    }

    public void Add(Color color)
    {
        _colors.Add(color);
    }

    public void Remove(int index)
    {
        if (index >= 0 &&
            index < _colors.Count)
        {
            _colors.RemoveAt(index);
        }
    }
}

public static class MaterialDB
{
    private static readonly Dictionary<MaterialType, ColorSamples> _colors;
    private static readonly Random _r;

    static MaterialDB()
    {
        _r = new Random();
        _colors = new Dictionary<MaterialType, ColorSamples>();

        var voidColor = new ColorSamples();
        voidColor.Add(Color.Transparent);
        _colors.Add(MaterialType.Empty, voidColor);

        var sandColors = new ColorSamples();
        sandColors.Add(new Color(234, 191, 125));
        sandColors.Add(new Color(255, 170, 114));
        sandColors.Add(new Color(219, 154, 89));
        _colors.Add(MaterialType.Sand, sandColors);
        
        var waterColors = new ColorSamples();
        waterColors.Add(new Color(35, 137, 218));
        waterColors.Add(new Color(33, 125, 196));
        waterColors.Add(new Color(34, 130, 204));
        _colors.Add(MaterialType.Water, waterColors);

        var stoneColors = new ColorSamples();
        stoneColors.Add(new Color(136, 140, 141));
        stoneColors.Add(new Color(123, 126, 127));
        stoneColors.Add(new Color(128, 131, 132));
        _colors.Add(MaterialType.Stone, stoneColors);
        
        var wanderer = new ColorSamples();
        wanderer.Add(new Color(0, 255, 255));
        _colors.Add(MaterialType.Wanderer, wanderer);
        
    }
    
    public static Color GetColor(MaterialType type, int lifeTime)
    {
        return _colors[type][_r.Next(0, _colors[type].Count)];
    }
}
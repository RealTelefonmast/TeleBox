namespace TeleBox.Scenes.Materials;

[Flags]
public enum MaterialType : int
{
    Empty, 
    Sand, 
    Water,
    Stone, 
    Oil, //flammable
    Fire, 
    Steam,
    Smoke, 
    Ember, 
    Coal, //flammable
    Wood, //flammable
    Acid,
    Lava,
    Titan,
    Obsidian,
    Ash,
    Methane, //flammable
    BurningGas,
    Ice,
    Plant, //flammable
    Dirt,
    Seed, //flammable
    Virus,
}

[Flags]
public enum MaterialAttributes : int
{
    None = 0,
    Flammable = 1 << 0,
    Liquid = 1 << 1,
    Gas = 1 << 2,
    Solid = 1 << 3,
    Burning = 1 << 4,
    Movable = 1 << 5,
    Static = 1 << 6
}

public static class MaterialProperties
{
    public static Dictionary<MaterialType, MaterialAttributes> Properties = new Dictionary<MaterialType, MaterialAttributes>
    {
        { MaterialType.Empty, MaterialAttributes.None },
        { MaterialType.Sand, MaterialAttributes.Solid | MaterialAttributes.Movable },
        { MaterialType.Water, MaterialAttributes.Liquid | MaterialAttributes.Movable },
        { MaterialType.Stone, MaterialAttributes.Solid | MaterialAttributes.Static },
        { MaterialType.Oil, MaterialAttributes.Liquid | MaterialAttributes.Movable | MaterialAttributes.Flammable },
        { MaterialType.Fire, MaterialAttributes.Burning | MaterialAttributes.Static },
        { MaterialType.Steam, MaterialAttributes.Gas | MaterialAttributes.Movable },
        { MaterialType.Smoke, MaterialAttributes.Gas | MaterialAttributes.Movable },
        { MaterialType.Ember, MaterialAttributes.Burning | MaterialAttributes.Gas | MaterialAttributes.Movable },
        { MaterialType.Coal, MaterialAttributes.Solid | MaterialAttributes.Movable | MaterialAttributes.Flammable },
        { MaterialType.Wood, MaterialAttributes.Solid | MaterialAttributes.Movable | MaterialAttributes.Flammable },
        { MaterialType.Acid, MaterialAttributes.Liquid | MaterialAttributes.Movable },
        { MaterialType.Lava, MaterialAttributes.Liquid | MaterialAttributes.Movable },
        { MaterialType.Titan, MaterialAttributes.Solid | MaterialAttributes.Movable },
        { MaterialType.Obsidian, MaterialAttributes.Solid | MaterialAttributes.Movable },
        { MaterialType.Ash, MaterialAttributes.Solid | MaterialAttributes.Static },
        { MaterialType.Methane, MaterialAttributes.Liquid | MaterialAttributes.Static | MaterialAttributes.Flammable },
        { MaterialType.BurningGas, MaterialAttributes.Burning | MaterialAttributes.Gas | MaterialAttributes.Static },
        { MaterialType.Ice, MaterialAttributes.Solid | MaterialAttributes.Movable },
        { MaterialType.Plant, MaterialAttributes.Solid | MaterialAttributes.Movable | MaterialAttributes.Flammable },
        { MaterialType.Dirt, MaterialAttributes.Solid | MaterialAttributes.Static },
        { MaterialType.Seed, MaterialAttributes.Solid | MaterialAttributes.Movable | MaterialAttributes.Flammable },
        { MaterialType.Virus, MaterialAttributes.Solid | MaterialAttributes.Movable },
    };

    public static bool IsLiquid(MaterialType type)
    {
        return (Properties[type] & MaterialAttributes.Liquid) == MaterialAttributes.Liquid;
    }
}
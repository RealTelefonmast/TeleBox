using System.Runtime.InteropServices;
using SFML.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public enum MaterialType : byte
{
    RigidBody = 8,
    Empty = 0,
    Sand = 1,
    Water = 2,
    Wanderer = 3,
    Smoke,
    Stone
}

public unsafe struct Particle
{
    public MaterialType id;
    public float lifeTime;
    public Vector2 velocity;
    public Color color;
    public bool hasBeenUpdated;

    public Particle(MaterialType id)
    {
        this.id = id;
        lifeTime = 1;
        velocity = Vector2.Zero;
        color = MaterialDB.GetColor(id,0);
        hasBeenUpdated = false;
    }
    
    public Particle()
    {
        id = MaterialType.Empty;
        lifeTime = 1;
        velocity = Vector2.Zero;
        color = Color.White;
        hasBeenUpdated = false;
    }

    public static Particle Empty => new Particle
    {
        id = MaterialType.Empty,
        lifeTime = 0,
        velocity = default,
        color = Color.Transparent,
        hasBeenUpdated = false
    };
}
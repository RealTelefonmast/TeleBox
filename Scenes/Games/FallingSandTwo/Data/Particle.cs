using SFML.Graphics;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public enum MaterialType : byte
{
    Empty = 0,
    Sand = 1,
    Water = 2,
}

public struct Particle
{
    public MaterialType id;
    public float lifeTime;
    public Vector2 velocity;
    public Color color;
    public bool hasBeenUpdated;

    public Particle()
    {
        id = MaterialType.Empty;
        lifeTime = 1;
        velocity = Vector2.Zero;
        color = Color.White;
        hasBeenUpdated = false;
    }
}
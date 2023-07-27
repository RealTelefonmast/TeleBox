

using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public static class Const
{
    public const float gravityScale = 5.0f;
    public static readonly Vector2 Gravity = new(0, 10.0f * gravityScale);
    public const float PI = 3.141592741f;
    public static float EPSILON = 0.0001f;
    public const float dt = 1.0f / 60.0f;
}
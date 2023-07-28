using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public unsafe struct RigidBody
{
    public Vector2 position;
    public Vector2 velocity;
    public float angularVelocity;
    public float torque;
    public float orient; // radians
    public Vector2 force;
    public float I; // moment of inertia
    public float iI; // inverse inertia
    public float m; // mass
    public float im; // inverse mass
    public float staticFriction;
    public float dynamicFriction;
    public float restitution;

    public PixelShape* shape;

    public RigidBody(PixelShape* pixelShape, int x, int y)
    {
        position = new Vector2(x, y);
        velocity = Vector2.Zero;
        angularVelocity = 0;
        torque = 0;
        orient = Rand.Range(-Const.PI, Const.PI);
        force = Vector2.Zero;
        staticFriction = 0.5f;
        dynamicFriction = 0.3f;
        restitution = 0.2f;
    }

    public void Init(RigidBody* body, PixelShape* pixelShape)
    {
        shape = pixelShape->Clone();
        shape->Body = body;
        shape->Initialize();
    }

    public void ApplyForce(Vector2 f)
    {
        force += f;
    }

    public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
    {
        velocity += impulse * im;
        angularVelocity += iI * Vector2.Cross(contactVector, impulse);
    }

    public void SetStatic()
    {
        I = 0f;
        iI = 0f;
        m = 0f;
        im = 0f;
    }
    
    public void SetOrient(float radians)
    {
        orient = radians;
    }
}
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public class RigidBody
{
    public Vector2 position;
    public Vector2 velocity;
    public float angularVelocity;
    public float torque;
    public float orient; // radians
    public Vector2 force;
    public float I;  // moment of inertia
    public float iI; // inverse inertia
    public float m;  // mass
    public float im; // inverse mass
    public float staticFriction;
    public float dynamicFriction;
    public float restitution;
    
    public IShape shape;
    
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
using SFML.Graphics;
using SFML.System;
using TeleBox.Engine.Data.Physics.Data;
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics;

public unsafe class Physics
{
    float m_dt =  1.0f / 60.0f;
    uint m_iterations = 10;
    
    //
    public RigidBody[] _bodies = Array.Empty<RigidBody>();
    public Manifold[] _contacts = Array.Empty<Manifold>();
    
    public RigidBody* Add(PixelShape* shape, int x, int y)
    {
        var body = new RigidBody(shape, x, y);
        body.Init(&body, shape);
        
        // Adding to array (resizes and appends the pointer)
        var temp = _bodies;
        Array.Resize<RigidBody>(ref temp, temp.Length + 1);
        temp[^1] = body;
        _bodies = temp;
        return &body;
    }

    private Manifold* AddManifold(Manifold manifold)
    {
        var temp = _contacts;
        Array.Resize<Manifold>(ref temp, temp.Length + 1);
        temp[^1] = manifold;
        _contacts = temp;
        return &manifold;
    }

    public void Clear()
    {
        
    }
    
    public void UpdateVerlet()
    {
        _contacts = Array.Empty<Manifold>();
        fixed (RigidBody* bodies = _bodies)
        {
            fixed (Manifold* contacts = _contacts)
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    RigidBody* bodyA = &bodies[i];

                    for (int j = i + 1; j < _bodies.Length; ++j)
                    {
                        RigidBody* bodyB = &bodies[j];
                        if (bodyA->im == 0 && bodyB->im == 0)
                            continue;
                        Manifold m = new Manifold(bodyA, bodyB);
                        m.Solve();
                        if (m.contact_count > 0)
                            AddManifold(m);
                    }
                }

                // Integrate forces
                for (int i = 0; i < _bodies.Length; ++i)
                {
                    IntegrateForces(&bodies[i], m_dt);
                }

                // Initialize collision
                for (int i = 0; i < _contacts.Length; ++i)
                {
                    (&contacts[i])->Initialize();
                }

                // Solve collisions
                for (int j = 0; j < m_iterations; ++j)
                {
                    for (int i = 0; i < _contacts.Length; ++i)
                    {
                        (&contacts[i])->ApplyImpulse();
                    }
                }

                // Integrate velocities
                for (int i = 0; i < _bodies.Length; ++i)
                {
                    IntegrateVelocity(&bodies[i], m_dt);
                }

                // Correct positions
                for (int i = 0; i < _contacts.Length; ++i)
                {
                    (&contacts[i])->PositionalCorrection();
                }

                // Clear all forces
                for (int i = 0; i < _bodies.Length; ++i)
                {
                    var temp = &bodies[i];
                    temp->force = new Vector2(0, 0);
                    temp->torque = 0;
                }
            }
        }
    }

    
    void IntegrateVelocity(RigidBody* b, float dt)
    {
        if (b->im == 0.0f) return;

        b->position += b->velocity * dt;
        b->orient += b->angularVelocity * dt;
        b->SetOrient(b->orient);
        IntegrateForces(b, dt);
    }
    
    void IntegrateForces(RigidBody* b, float dt)
    {
        if (b->im == 0.0f) return;

        b->velocity += (b->force * b->im + Const.Gravity) * (dt / 2.0f);
        b->angularVelocity += b->torque * b->iI * (dt / 2.0f);
    }

    public void Render(RenderArgs args)
    {
        for(uint i = 0; i < _bodies.Length; ++i)
        {
            var b = _bodies[i];
            DrawShape(args, b);
        }
        
        for(uint i = 0; i < _contacts.Length; ++i)
        {
            Manifold m = _contacts[i];
            for(int j = 0; j < m.contact_count; ++j)
            {
                Vector2 c = m.Contacts[j];
                SFML.Graphics.CircleShape circle = new SFML.Graphics.CircleShape(2);
                circle.Position = new SFML.System.Vector2f(c.x, c.y);
                args.Target.Draw(circle);
            }
        }
    }

    public void DrawShape(RenderArgs args, RigidBody body)
    {
        var color = new CircleShape(5);
        color.Position = new Vector2f(body.position.x, body.position.y);
        color.FillColor = Color.Cyan;
        args.Target.Draw(color);
    }

}
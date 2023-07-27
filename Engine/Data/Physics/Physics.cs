using TeleBox.Engine.Data.Physics.Data;
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics;

public class Physics
{
    float m_dt;
    uint m_iterations;
    public List<RigidBody> _bodies;
    public List<Manifold> _contacts;


    public void UpdateVerlet()
    {
        _contacts.Clear( );
        for(int i = 0; i < _bodies.Count; i++)
        {
            RigidBody bodyA = _bodies[i];

            for(int j = i + 1; j < _bodies.Count; ++j)
            {
                RigidBody bodyB = _bodies[j];
                if(bodyA.im == 0 && bodyB.im == 0)
                    continue;
                Manifold m = new Manifold(bodyA, bodyB);
                m.Solve( );
                if(m.contact_count > 0)
                    _contacts.Add( m );
            }
        }
        
        // Integrate forces
        for (int i = 0; i < _bodies.Count; ++i)
        {
            _bodies[i] = IntegrateForces(_bodies[i], m_dt);
        }

        // Initialize collision
        for (int i = 0; i < _contacts.Count; ++i)
        {
            var contact = _contacts[i];
            _contacts[i] = contact.Initialize();
        }

        // Solve collisions
        for(int j = 0; j < m_iterations; ++j)
        for (int i = 0; i < _contacts.Count; ++i)
        {
            var contact = _contacts[i];
            _contacts[i] = contact.ApplyImpulse();
        }

        // Integrate velocities
        for (int i = 0; i < _bodies.Count; ++i)
        {
            _bodies[i] = IntegrateVelocity(_bodies[i], m_dt);
        }

        // Correct positions
        for (int i = 0; i < _contacts.Count; ++i)
        {
            var contact = _contacts[i];
            _contacts[i] = contact.PositionalCorrection();
        }

        // Clear all forces
        for(int i = 0; i < _bodies.Count; ++i)
        {
            RigidBody temp = _bodies[i];
            temp.force = new(0, 0);
            temp.torque = 0;
            _bodies[i] = temp;
        }
        
    }

    RigidBody IntegrateForces(RigidBody b, float dt)
    {
        if (b.im == 0.0f) return b;

        b.velocity += (b.force * b.im + Const.Gravity) * (dt / 2.0f);
        b.angularVelocity += b.torque * b.iI * (dt / 2.0f);

        return b;
    }

    RigidBody IntegrateVelocity(RigidBody b, float dt)
    {
        if (b.im == 0.0f) return b;

        b.position += b.velocity * dt;
        b.orient += b.angularVelocity * dt;
        b.SetOrient(b.orient);
        IntegrateForces(b, dt);
        return b;
    }

}
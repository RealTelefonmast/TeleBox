using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public struct Manifold
{
    private RigidBody A;
    private RigidBody B;
    
    public uint contact_count; // Number of contacts that occured during collision
    
    private float penetration; // Depth of penetration from collision
    private Vector2 normal; // From A to B
    private Vector2[] contacts; // Points of contact during collision
    private float e; // Mixed restitution
    private float df; // Mixed dynamic friction
    private float sf; // Mixed static friction

    public Manifold(RigidBody bodyA, RigidBody bodyB)
    {
        A = bodyA;
        B = bodyB;
    }

    // Generate contact information
    public void Solve()
    {
        var shapeA = this.A.shape;
        var shapeB = this.B.shape;
        contact_count = 0;

        // Check for a separating axis with A's face planes
        uint faceA;
        float penetrationA = FindAxisLeastPenetration(faceA, shapeA, shapeB);
        if (penetrationA >= 0.0f)
            return;

        // Check for a separating axis with B's face planes
        uint faceB;
        float penetrationB = FindAxisLeastPenetration(faceB, shapeB, shapeA);
        if (penetrationB >= 0.0f)
            return;

        uint referenceIndex;
        bool flip; // Always point from a to b

        IShape RefPoly; // Reference
        IShape IncPoly; // Incident

        // Determine which shape contains reference face
        if (BiasGreaterThan(penetrationA, penetrationB))
        {
            RefPoly = shapeA;
            IncPoly = shapeB;
            referenceIndex = faceA;
            flip = false;
        }

        else
        {
            RefPoly = B;
            IncPoly = A;
            referenceIndex = faceB;
            flip = true;
        }

        // World space incident face
        Vec2 incidentFace[2];
        FindIncidentFace(incidentFace, RefPoly, IncPoly, referenceIndex);

        //        y
        //        ^  ->n       ^
        //      +---c ------posPlane--
        //  x < | i |\
        //      +---+ c-----negPlane--
        //             \       v
        //              r
        //
        //  r : reference face
        //  i : incident poly
        //  c : clipped point
        //  n : incident normal

        // Setup reference face vertices
        Vec2 v1 = RefPoly->m_vertices[referenceIndex];
        referenceIndex = referenceIndex + 1 == RefPoly->m_vertexCount ? 0 : referenceIndex + 1;
        Vec2 v2 = RefPoly->m_vertices[referenceIndex];

        // Transform vertices to world space
        v1 = RefPoly->u * v1 + RefPoly->body->position;
        v2 = RefPoly->u * v2 + RefPoly->body->position;

        // Calculate reference face side normal in world space
        Vec2 sidePlaneNormal = (v2 - v1);
        sidePlaneNormal.Normalize();

        // Orthogonalize
        Vec2 refFaceNormal(sidePlaneNormal.y, -sidePlaneNormal.x);

        // ax + by = c
        // c is distance from origin
        real refC = Dot(refFaceNormal, v1);
        real negSide = -Dot(sidePlaneNormal, v1);
        real posSide = Dot(sidePlaneNormal, v2);

        // Clip incident face to reference face side planes
        if (Clip(-sidePlaneNormal, negSide, incidentFace) < 2)
            return; // Due to floating point error, possible to not have required points

        if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
            return; // Due to floating point error, possible to not have required points

        // Flip
        m->normal = flip ? -refFaceNormal : refFaceNormal;

        // Keep points behind reference face
        uint32 cp = 0; // clipped points behind reference face
        real separation = Dot(refFaceNormal, incidentFace[0]) - refC;
        if (separation <= 0.0f)
        {
            m->contacts[cp] = incidentFace[0];
            m->penetration = -separation;
            ++cp;
        }
        else
            m->penetration = 0;

        separation = Dot(refFaceNormal, incidentFace[1]) - refC;
        if (separation <= 0.0f)
        {
            m->contacts[cp] = incidentFace[1];

            m->penetration += -separation;
            ++cp;

            // Average penetration
            m->penetration /= (real)cp;
        }

        m->contact_count = cp;
    }

    public static bool BiasGreaterThan(float a, float b)
    {
        const float k_biasRelative = 0.95f;
        const float k_biasAbsolute = 0.01f;
        return a >= b * k_biasRelative + a * k_biasAbsolute;
    }
    
    private float FindAxisLeastPenetration(ref int faceIndex, ref IShape A, ref IShape B)
    {
        float bestDistance = -float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < A.m_vertexCount; ++i)
        {
            // Retrieve a face normal from A
            Vector2 n = A.m_normals[i];
            Vector2 nw = A.u * n;

            // Transform face normal into B's model space
            Matrix2x2 buT = B.u.Transpose();
            n = buT * nw;

            // Retrieve support point from B along -n
            Vector2 s = B.GetSupport(-n);

            // Retrieve vertex on face from A, transform into
            // B's model space
            Vector2 v = A.m_vertices[i];
            v = A.u * v + A.body.position;
            v -= B.body->position;
            v = buT * v;

            // Compute penetration distance (in B's model space)
            float d = Vector2.Dot(n, s - v);

            // Store greatest distance
            if (d > bestDistance)
            {
                bestDistance = d;
                bestIndex = i;
            }
        }

        faceIndex = bestIndex;
        return bestDistance;
    }

    private void FindIncidentFace(Vector2 v, IShape RefPoly, IShape IncPoly, uint referenceIndex)
    {
        Vector2 referenceNormal = RefPoly.m_normals[referenceIndex];

        // Calculate normal in incident's frame of reference
        referenceNormal = RefPoly.u * referenceNormal; // To world space
        referenceNormal = IncPoly.u.Transpose() * referenceNormal; // To incident's model space

        // Find most anti-normal face on incident polygon
        int incidentFace = 0;
        float minDot = float.MaxValue;
        for (uint i = 0; i < IncPoly.m_vertexCount; ++i)
        {
            float dot = Vector2.Dot(referenceNormal, IncPoly.m_normals[i]);
            if (dot < minDot)
            {
                minDot = dot;
                incidentFace = i;
            }
        }

        // Assign face vertices for incidentFace
        v[0] = IncPoly.u * IncPoly.m_vertices[incidentFace] + IncPoly.body.position;
        incidentFace = incidentFace + 1 >= (int)IncPoly.m_vertexCount ? 0 : incidentFace + 1;
        v[1] = IncPoly.u * IncPoly.m_vertices[incidentFace] + IncPoly.body.position;
    }

    private uint Clip( Vector2 n, float c, Vector2[] face)
    {
      uint sp = 0;
      Vector2[] @out = new Vector2[2]{
        face[0],
        face[1]
      };

      // Retrieve distances from each endpoint to the line
      // d = ax + by - c
      float d1 = Vector2.Dot( n, face[0] ) - c;
      float d2 = Vector2.Dot( n, face[1] ) - c;

      // If negative (behind plane) clip
      if(d1 <= 0.0f) @out[sp++] = face[0];
      if(d2 <= 0.0f) @out[sp++] = face[1];
      
      // If the points are on different sides of the plane
      if(d1 * d2 < 0.0f) // less than to ignore -0.0f
      {
        // Push interesection point
        float alpha = d1 / (d1 - d2);
        @out[sp] = face[0] + alpha * (face[1] - face[0]);
        ++sp;
      }

      // Assign our new converted values
      face[0] = @out[0];
      face[1] = @out[1];

      //assert( sp != 3 );

      return sp;
    }

    // Precalculations for impulse solving
    public Manifold Initialize()
    {
        // Calculate average restitution
        e = Math.Min(A.restitution, B.restitution);

        // Calculate static and dynamic friction
        sf = MathF.Sqrt(A.staticFriction * A.staticFriction);
        df = MathF.Sqrt(A.dynamicFriction * A.dynamicFriction);

        for (int i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            Vector2 ra = contacts[i] - A.position;
            Vector2 rb = contacts[i] - B.position;

            Vector2 rv = B.velocity + Vector2.Cross(B.angularVelocity, rb) -
                         A.velocity - Vector2.Cross(A.angularVelocity, ra);


            // Determine if we should perform a resting collision or not
            // The idea is if the only thing moving this object is gravity,
            // then the collision should be performed without any restitution
            if (rv.LengthSqr < (Const.Gravity * Const.dt).LengthSqr + Const.EPSILON)
                e = 0.0f;
        }
        return this;
    }

    // Solve impulse and apply
    public Manifold ApplyImpulse()
    {
        // Early out and positional correct if both objects have infinite mass
        if (A.im + B.im == 0) //  if(Equal( A->im + B->im, 0 ))
        {
            InfiniteMassCorrection();
            return this;
        }

        for (int i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            Vector2 ra = contacts[i] - A.position;
            Vector2 rb = contacts[i] - B.position;

            // Relative velocity
            Vector2 rv = B.velocity + Vector2.Cross(B.angularVelocity, rb) -
                         A.velocity - Vector2.Cross(A.angularVelocity, ra);

            // Relative velocity along the normal
            float contactVel = Vector2.Dot(rv, normal);

            // Do not resolve if velocities are separating
            if (contactVel > 0) return this;

            float raCrossN = Vector2.Cross(ra, normal);
            float rbCrossN = Vector2.Cross(rb, normal);
            float invMassSum = A.im + B.im + MathF.Sqrt(raCrossN) * A.iI + MathF.Sqrt(rbCrossN) * B.iI;

            // Calculate impulse scalar
            float j = -(1.0f + e) * contactVel;
            j /= invMassSum;
            j /= contact_count;

            // Apply impulse
            Vector2 impulse = normal * j;
            A.ApplyImpulse(-impulse, ra);
            B.ApplyImpulse(impulse, rb);

            // Friction impulse
            rv = B.velocity + Vector2.Cross(B.angularVelocity, rb) -
                 A.velocity - Vector2.Cross(A.angularVelocity, ra);

            Vector2 t = rv - (normal * Vector2.Dot(rv, normal));
            t = t.Normalized;

            // j tangent magnitude
            float jt = -(Vector2.Dot(rv, t));
            jt /= invMassSum;
            jt /= contact_count;

            // Don't apply tiny friction impulses
            if (jt == 0.0f) return this;

            // Coulumb's law
            Vector2 tangentImpulse;
            if (MathF.Abs(jt) < j * sf)
                tangentImpulse = t * jt;
            else
                tangentImpulse = t * -j * df;

            // Apply friction impulse
            A.ApplyImpulse(-tangentImpulse, ra);
            B.ApplyImpulse(tangentImpulse, rb);
        }

        return this;
    }

    // Naive correction of positional penetration
    public Manifold PositionalCorrection()
    {
        const float k_slop = 0.05f; // Penetration allowance
        const float percent = 0.4f; // Penetration percentage to correct
        var correction = (normal * (MathF.Max( penetration - k_slop, 0.0f ) / (A.im + B.im))) * percent;
        A.position -= correction * A.im;
        B.position += correction * B.im;
        return this;
    }

    public void InfiniteMassCorrection()
    {
        A.velocity = new ( 0, 0 );
        B.velocity = new ( 0, 0 );
    }
}
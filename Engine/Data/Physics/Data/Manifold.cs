using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public static class UnsafeTools
{
    public static unsafe T* GetPointer<T>(this T value)
    {
        return &value;
    }
}

public unsafe struct Manifold
{
    private RigidBody* A;
    private RigidBody* B;
    
    public uint contact_count; // Number of contacts that occured during collision
    
    private float penetration; // Depth of penetration from collision
    private Vector2 normal; // From A to B
    private fixed float contactsData[4];
    private float e; // Mixed restitution
    private float df; // Mixed dynamic friction
    private float sf; // Mixed static friction

    // Points of contact during collision
    public Vector2* Contacts
    {
        get
        {
            fixed (float* cont = contactsData)
                return (Vector2*)cont;
        }
    }

    public Manifold(RigidBody* bodyA, RigidBody* bodyB)
    {
        A = bodyA;
        B = bodyB;
    }

    // Generate contact information
    public void Solve()
    {
        PixelShape* shapeA = A->shape;
        PixelShape* shapeB = B->shape;
        contact_count = 0;

        // Check for a separating axis with A's face planes
        uint faceA;
        float penetrationA = FindAxisLeastPenetration(&faceA, shapeA, shapeB);
        if (penetrationA >= 0.0f)
            return;

        // Check for a separating axis with B's face planes
        uint faceB;
        float penetrationB = FindAxisLeastPenetration(&faceB, shapeB, shapeA);
        if (penetrationB >= 0.0f)
            return;

        uint referenceIndex;
        bool flip; // Always point from a to b

        PixelShape* RefPoly; // Reference
        PixelShape* IncPoly; // Incident

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
            RefPoly = shapeB;
            IncPoly = shapeA;
            referenceIndex = faceB;
            flip = true;
        }

        // World space incident face
        fixed (float* incidentFaceData = stackalloc float[4])
        {
            Vector2* incidentFace = (Vector2*)incidentFaceData;
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
            Vector2 v1 = RefPoly->Vertices[referenceIndex];
            referenceIndex = referenceIndex + 1 == RefPoly->m_VertexCount ? 0 : referenceIndex + 1;
            Vector2 v2 = RefPoly->Vertices[referenceIndex];

            // Transform vertices to world space
            v1 = RefPoly->Matrix * v1 + RefPoly->Body->position;
            v2 = RefPoly->Matrix * v2 + RefPoly->Body->position;

            // Calculate reference face side normal in world space
            Vector2 sidePlaneNormal = (v2 - v1);
            sidePlaneNormal = sidePlaneNormal.Normalized;

            // Orthogonalize
            Vector2 refFaceNormal = new(sidePlaneNormal.y, -sidePlaneNormal.x);

            // ax + by = c
            // c is distance from origin
            float refC = Vector2.Dot(refFaceNormal, v1);
            float negSide = -Vector2.Dot(sidePlaneNormal, v1);
            float posSide = Vector2.Dot(sidePlaneNormal, v2);

            // Clip incident face to reference face side planes
            if (Clip(-sidePlaneNormal, negSide, incidentFace) < 2)
                return; // Due to floating point error, possible to not have required points

            if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
                return; // Due to floating point error, possible to not have required points

            // Flip
            normal = flip ? -refFaceNormal : refFaceNormal;

            // Keep points behind reference face
            uint contactPoints = 0; // clipped points behind reference face
            float separation = Vector2.Dot(refFaceNormal, incidentFace[0]) - refC;
            if (separation <= 0.0f)
            {
                Contacts[contactPoints] = incidentFace[0];
                penetration = -separation;
                ++contactPoints;
            }
            else
                penetration = 0;

            separation = Vector2.Dot(refFaceNormal, incidentFace[1]) - refC;
            if (separation <= 0.0f)
            {
                Contacts[contactPoints] = incidentFace[1];

                penetration += -separation;
                ++contactPoints;

                // Average penetration
                penetration /= contactPoints;
            }

            contact_count = contactPoints;
        }
    }

    public static bool BiasGreaterThan(float a, float b)
    {
        const float kBiasRelative = 0.95f;
        const float kBiasAbsolute = 0.01f;
        return a >= b * kBiasRelative + a * kBiasAbsolute;
    }

    private float FindAxisLeastPenetration(uint* faceIndex, PixelShape* shapeA, PixelShape* shapeB)
    {
        var bestDistance = -float.MaxValue;
        uint bestIndex = 0;

        for (uint i = 0; i < shapeA->m_VertexCount; i++)
        {
            // Retrieve a face normal from A
            var n = shapeA->Normals[i];
            var nw = shapeA->Matrix * n;

            // Transform face normal into B's model space
            var buT = shapeB->Matrix.Transpose();
            n = buT * nw;

            // Retrieve support point from B along -n
            var s = shapeB->GetSupport(-n);

            // Retrieve vertex on face from A, transform into
            // B's model space
            var v = shapeA->Vertices[i];
            v = shapeA->Matrix * v + shapeA->Body->position;
            v -= shapeB->Body->position;
            v = buT * v;

            // Compute penetration distance (in B's model space)
            var d = Vector2.Dot(n, s - v);

            // Store greatest distance
            if (d > bestDistance)
            {
                bestDistance = d;
                bestIndex = i;
            }
        }
        
        faceIndex[0] = bestIndex;
        return bestDistance;
    }

    private unsafe void FindIncidentFace(Vector2* v, PixelShape* RefPoly, PixelShape* IncPoly, uint referenceIndex)
    {
        Vector2 referenceNormal = RefPoly->Normals[referenceIndex];

        // Calculate normal in incident's frame of reference
        referenceNormal = RefPoly->Matrix * referenceNormal; // To world space
        referenceNormal = IncPoly->Matrix.Transpose() * referenceNormal; // To incident's model space

        // Find most anti-normal face on incident polygon
        uint incidentFace = 0;
        float minDot = float.MaxValue;
        for (uint i = 0; i < IncPoly->m_VertexCount; ++i)
        {
            float dot = Vector2.Dot(referenceNormal, IncPoly->Normals[i]);
            if (dot < minDot)
            {
                minDot = dot;
                incidentFace = i;
            }
        }

        // Assign face vertices for incidentFace
        v[0] = IncPoly->Matrix * IncPoly->Vertices[incidentFace] + IncPoly->Body->position;
        incidentFace = incidentFace + 1 >= (int)IncPoly->m_VertexCount ? 0 : incidentFace + 1;
        v[1] = IncPoly->Matrix * IncPoly->Vertices[incidentFace] + IncPoly->Body->position;
    }

    private uint Clip( Vector2 n, float c, Vector2* face)
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
        @out[sp] = face[0] + ((face[1] - face[0]) * alpha);
        ++sp;
      }

      // Assign our new converted values
      face[0] = @out[0];
      face[1] = @out[1];

      //assert( sp != 3 );

      return sp;
    }

    // Precalculations for impulse solving
    public void Initialize()
    {
        // Calculate average restitution
        e = Math.Min(A->restitution, B->restitution);

        // Calculate static and dynamic friction
        sf = MathF.Sqrt(A->staticFriction * A->staticFriction);
        df = MathF.Sqrt(A->dynamicFriction * A->dynamicFriction);

        for (var i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            var ra = Contacts[i] - A->position;
            var rb = Contacts[i] - B->position;

            var rv = B->velocity + Vector2.Cross(B->angularVelocity, rb) -
                     A->velocity - Vector2.Cross(A->angularVelocity, ra);
            
            // Determine if we should perform a resting collision or not
            // The idea is if the only thing moving this object is gravity,
            // then the collision should be performed without any restitution
            if (rv.LengthSqr < (Const.Gravity * Const.dt).LengthSqr + Const.EPSILON)
                e = 0.0f;
        }
    }

    // Solve impulse and apply
    public void ApplyImpulse()
    {
        // Early out and positional correct if both objects have infinite mass
        if (A->im + B->im == 0) //  if(Equal( A->im + B->im, 0 ))
        {
            InfiniteMassCorrection();
            return;
        }

        for (int i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            Vector2 ra = Contacts[i] - A->position;
            Vector2 rb = Contacts[i] - B->position;

            // Relative velocity
            Vector2 rv = B->velocity + Vector2.Cross(B->angularVelocity, rb) -
                         A->velocity - Vector2.Cross(A->angularVelocity, ra);

            // Relative velocity along the normal
            float contactVel = Vector2.Dot(rv, normal);

            // Do not resolve if velocities are separating
            if (contactVel > 0) return;

            float raCrossN = Vector2.Cross(ra, normal);
            float rbCrossN = Vector2.Cross(rb, normal);
            float invMassSum = A->im + B->im + MathF.Sqrt(raCrossN) * A->iI + MathF.Sqrt(rbCrossN) * B->iI;

            // Calculate impulse scalar
            float j = -(1.0f + e) * contactVel;
            j /= invMassSum;
            j /= contact_count;

            // Apply impulse
            Vector2 impulse = normal * j;
            A->ApplyImpulse(-impulse, ra);
            B->ApplyImpulse(impulse, rb);

            // Friction impulse
            rv = B->velocity + Vector2.Cross(B->angularVelocity, rb) -
                 A->velocity - Vector2.Cross(A->angularVelocity, ra);

            Vector2 t = rv - (normal * Vector2.Dot(rv, normal));
            t = t.Normalized;

            // j tangent magnitude
            float jt = -(Vector2.Dot(rv, t));
            jt /= invMassSum;
            jt /= contact_count;

            // Don't apply tiny friction impulses
            if (jt == 0.0f) return;

            // Coulumb's law
            Vector2 tangentImpulse;
            if (MathF.Abs(jt) < j * sf)
                tangentImpulse = t * jt;
            else
                tangentImpulse = t * -j * df;

            // Apply friction impulse
            A->ApplyImpulse(-tangentImpulse, ra);
            B->ApplyImpulse(tangentImpulse, rb);
        }
    }

    // Naive correction of positional penetration
    public void PositionalCorrection()
    {
        const float k_slop = 0.05f; // Penetration allowance
        const float percent = 0.4f; // Penetration percentage to correct
        var correction = (normal * (MathF.Max( penetration - k_slop, 0.0f ) / (A->im + B->im))) * percent;
        A->position -= correction * A->im;
        B->position += correction * B->im;
    }

    public void InfiniteMassCorrection()
    {
        A->velocity = new ( 0, 0 );
        B->velocity = new ( 0, 0 );
    }
}
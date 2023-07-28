using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public unsafe struct PixelShape
{
    public int pixelCount;
    public int m_VertexCount;
    private fixed float vertices[128];
    private fixed float normals[128];

    public Matrix2x2 Matrix { get; set; }
    public RigidBody* Body { get; set; }

    public Vector2* Vertices
    {
        get
        {
            fixed (float* p = vertices)
            {
                return (Vector2*)p;
            }
        }
    }

    public Vector2* Normals
    {
        get
        {
            fixed (float* p = normals)
            {
                return (Vector2*)p;
            }
        }
    }

    
    public void Initialize( )
    {
        ComputeMass( 1.0f );
    }

    public PixelShape* Clone()
    {
        var newShape = new PixelShape();
        PixelShape* shape = &newShape;
        shape->Matrix = this.Matrix;
        for(uint i = 0; i < m_VertexCount; ++i)
        {
            shape->Vertices[i] = Vertices[i];
            shape->Normals[i] = Normals[i];
        }
        shape->m_VertexCount = m_VertexCount;
        return shape;
    }

    
    public Vector2 GetSupport(Vector2 dir)
    {
        var bestProjection = -float.MaxValue;
        var bestVertex = Vector2.Zero;

        for (uint i = 0; i < m_VertexCount; ++i)
        {
            Vector2 v = Vertices[i];
            float projection = Vector2.Dot(v, dir);

            if (projection > bestProjection)
            {
                bestVertex = v;
                bestProjection = projection;
            }
        }

        return bestVertex;
    }
    
    private void ComputeMass(float f)
    {
        Body->m = Const.PI * 5 * 5 * f;
        Body->im = (Body->m > 0) ? 1.0f / Body->m : 0.0f;
        Body->I = Body->m * 5 * 5;
        Body->iI = (Body->I > 0) ? 1.0f / Body->I : 0.0f;
    }

    public void Set(Vector2* vertices, int count)
    {
        // No hulls with less than 3 vertices (ensure actual polygon)
        count = Math.Min(count, 32);
        // Find the right most point on the hull
        int rightMost = 0;
        float highestXCoord = vertices[0].x;
        for (int i = 1; i < count; ++i)
        {
            float x = vertices[i].x;
            if (x > highestXCoord)
            {
                highestXCoord = x;
                rightMost = i;
            }

            // If matching x then take farthest negative y
            else if (x == highestXCoord)
                if (vertices[i].y < vertices[rightMost].y)
                    rightMost = i;
        }

        fixed (int* hull = stackalloc int[32])
        {
            int outCount = 0;
            int indexHull = rightMost;

            for (;;)
            {
                hull[outCount] = indexHull;

                // Search for next index that wraps around the hull
                // by computing cross products to find the most counter-clockwise
                // vertex in the set, given the previos hull index
                int nextHullIndex = 0;
                for (int i = 1; i < (int)count; ++i)
                {
                    // Skip if same coordinate as we need three unique
                    // points in the set to perform a cross product
                    if (nextHullIndex == indexHull)
                    {
                        nextHullIndex = i;
                        continue;
                    }

                    // Cross every set of three unique vertices
                    // Record each counter clockwise third vertex and add
                    // to the output hull
                    // See : http://www.oocities.org/pcgpe/math2d.html
                    Vector2 e1 = vertices[nextHullIndex] - vertices[hull[outCount]];
                    Vector2 e2 = vertices[i] - vertices[hull[outCount]];
                    float c = Vector2.Cross(e1, e2);
                    if (c < 0.0f)
                        nextHullIndex = i;

                    // Cross product is zero then e vectors are on same line
                    // therefor want to record vertex farthest along that line
                    if (c == 0.0f && e2.Magnitude > e1.Magnitude)
                        nextHullIndex = i;
                }

                ++outCount;
                indexHull = nextHullIndex;

                // Conclude algorithm upon wrap-around
                if (nextHullIndex == rightMost)
                {
                    m_VertexCount = outCount;
                    break;
                }
            }

            // Copy vertices into shape's vertices
            for (int i = 0; i < m_VertexCount; ++i)
                Vertices[i] = vertices[hull[i]];

            // Compute face normals
            for (int i1 = 0; i1 < m_VertexCount; ++i1)
            {
                int i2 = i1 + 1 < m_VertexCount ? i1 + 1 : 0;
                Vector2 face = Vertices[i2] - Vertices[i1];

                // Ensure no zero-length edges, because that's bad
                //assert(face.LenSqr() > EPSILON * EPSILON);

                // Calculate normal with 2D cross product between vector and scalar
                Normals[i1] = new Vector2(face.y, -face.x);
                Normals[i1] = Normals[i1].Normalized;
            }
        }
    }
}
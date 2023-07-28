using System.Runtime.InteropServices;
using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct Matrix2x2
{
    [FieldOffset(0)]
    public float m00;
    [FieldOffset(4)] 
    public float m01;
    [FieldOffset(8)]
    public float m10;
    [FieldOffset(12)]
    public float m11;
    
    [FieldOffset(0)]
    private fixed float matrix[4];
    
    public float this[int x, int y] => matrix[y * 2 + x];
    
    public Matrix2x2(float m00, float m01, float m10, float m11)
    {
        this.m00 = m00;
        this.m01 = m01;
        this.m10 = m10;
        this.m11 = m11;
    }

    public Matrix2x2(float radians)
    {
        var c = MathF.Cos(radians);
        var s = MathF.Sin(radians);

        m00 = c; m01 = -s;
        m10 = s; m11 = c;

    }
    
    public Vector2 Row1 
    { 
        get => new Vector2(m00, m01); 
        set 
        {
            m00 = value.x;
            m01 = value.y;
        }
    }

    public Vector2 Row2
    {
        get => new Vector2(m10, m11);
        set
        {
            m10 = value.x;
            m11 = value.y;
        }
    }

    Vector2 AxisX()
    {
        return new Vector2(m00, m10);
    }

    Vector2 AxisY()
    {
        return new Vector2(m01, m11);
    }

    public Vector2 Transform(Vector2 vector)
    {
        return new Vector2(m00 * vector.x + m01 * vector.y, m10 * vector.x + m11 * vector.y);
    }
    
    public Matrix2x2 Transpose()
    {
        return new Matrix2x2(m00, m10, m01, m11);
    }

    #region  Math

    public static Vector2 operator *(Matrix2x2 m, Vector2 v)
    {
        return m.Transform(v);
    }
    
    public static Matrix2x2 operator *(Matrix2x2 m, Matrix2x2 rhs)
    {
        return new Matrix2x2(
            m[0,0] * rhs[0,0] + m[0,1] * rhs[1,0],
            m[0,0] * rhs[0,1] + m[0,1] * rhs[1,1],
            m[1,0] * rhs[0,0] + m[1,1] * rhs[1,0],
            m[1,0] * rhs[0,1] + m[1,1] * rhs[1,1]
        );
    }

    #endregion
}
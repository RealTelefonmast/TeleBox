using TeleBox.Engine.Data.Primitive;

namespace TeleBox.Engine.Data.Physics.Data;

public struct Matrix2x2
{
    //           M00, M01, M10, M11;
    public float x0, x1, y0, y1;
    
    public Matrix2x2(Vector2 x, Vector2 y)
    {
        x0 = x.x;
        x1 = x.y;
        y0 = y.x;
        y1 = y.y;
    }
    
    public Matrix2x2(float x0, float x1, float y0, float y1)
    {
        x0 = x0;
        x1 = x1;
        y0 = y0;
        y1 = y1;
    }

    public Matrix2x2(float radians)
    {
        var c = MathF.Cos(radians);
        var s = MathF.Sin(radians);

        x0 = c; x1 = -s;
        y0 = s; y1 = c;

    }
    
    public Vector2 Row1 
    { 
        get => new Vector2(x0, x1); 
        set 
        {
            x0 = value.x;
            x1 = value.y;
        }
    }

    public Vector2 Row2
    {
        get => new Vector2(y0, y1);
        set
        {
            y0 = value.x;
            y1 = value.y;
        }
    }
    
    // Transforms a vector by this matrix
    public Vector2 Transform(Vector2 vector)
    {
        return new Vector2(x0 * vector.x + x1 * vector.y, y0 * vector.x + y1 * vector.y);
    }
}
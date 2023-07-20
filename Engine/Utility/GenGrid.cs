using TeleBox.UI;

namespace TeleBox.Engine.Utility;

public class GenGrid
{
    private List<IntVec2> CalculateTrajectory(int x0, int y0, int x1, int y1)
    {
        var trajectory = new List<IntVec2>();

        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var sx = (x0 < x1) ? 1 : -1;
        var sy = (y0 < y1) ? 1 : -1;
        var err = dx - dy;

        while (true)
        {
            trajectory.Add(new IntVec2(x0, y0));

            if ((x0 == x1) && (y0 == y1))
            {
                break;
            }

            (err, x0, y0) = UpdateParameters(err, dx, dy, x0, y0, sx, sy);
        }

        return trajectory;
    }

    private static (int err, int x0, int y0) UpdateParameters(int err, int dx, int dy, int x0, int y0, int sx, int sy)
    {
        var e2 = 2 * err;
        if (e2 > -dy)
        {
            err -= dy;
            x0 += sx;
        }

        if (e2 < dx)
        {
            err += dx;
            y0 += sy;
        }

        return (err, x0, y0);
    }
}
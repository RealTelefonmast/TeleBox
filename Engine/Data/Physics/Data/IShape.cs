using TeleBox.Scenes.Games.FallingSandTwo.Data;

namespace TeleBox.Engine.Data.Physics.Data;

public interface IShape
{
    // Properties for 'Body' and 'Matrix2' are suppose to be here,
    // but interfaces in C# do not support fields. Use properties instead.

    RigidBody Body { get; set; }
    double Radius { get; set; } // For circle shape
    Matrix2x2 U { get; set; } // Orientation matrix from model to world; For Polygon shape

    IShape Clone();
    void SetOrient(double radians);
}

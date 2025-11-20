using System.Numerics;

namespace TurretGame.Core.Utilities;

public static class CollisionDetection
{
    public static bool CircleToCircle(Vector2 position1, float radius1, Vector2 position2, float radius2)
    {
        float distance = Vector2.Distance(position1, position2);
        return distance < (radius1 + radius2);
    }
}

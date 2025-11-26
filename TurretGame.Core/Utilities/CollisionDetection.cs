using System;
using System.Numerics;

namespace TurretGame.Core.Utilities;

public static class CollisionDetection
{
    public static bool CircleToCircle(Vector2 position1, float radius1, Vector2 position2, float radius2)
    {
        float distance = Vector2.Distance(position1, position2);
        return distance < (radius1 + radius2);
    }

    /// <summary>
    /// Check collision between a circle and an axis-aligned square (AABB)
    /// </summary>
    /// <param name="circlePosition">Center of the circle</param>
    /// <param name="circleRadius">Radius of the circle</param>
    /// <param name="squarePosition">Center of the square</param>
    /// <param name="squareHalfSize">Half-width of the square</param>
    /// <returns>True if collision detected</returns>
    public static bool CircleToSquare(Vector2 circlePosition, float circleRadius, Vector2 squarePosition, float squareHalfSize)
    {
        // Find the closest point on the square to the circle
        float closestX = Math.Clamp(circlePosition.X, squarePosition.X - squareHalfSize, squarePosition.X + squareHalfSize);
        float closestY = Math.Clamp(circlePosition.Y, squarePosition.Y - squareHalfSize, squarePosition.Y + squareHalfSize);
        Vector2 closestPoint = new Vector2(closestX, closestY);

        // Check if the distance from the closest point to the circle center is less than the radius
        float distance = Vector2.Distance(circlePosition, closestPoint);
        return distance < circleRadius;
    }
}

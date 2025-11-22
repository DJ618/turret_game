using System.Numerics;

namespace TurretGame.Core.Entities;

public class Player
{
    public Vector2 Position { get; private set; }
    public float Speed { get; set; } = 400f;
    public float Radius { get; set; } = 20f;
    public int ResourceCount { get; private set; } = 0;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void SetPosition(Vector2 newPosition)
    {
        Position = newPosition;
    }

    public void CollectResource(int amount)
    {
        ResourceCount += amount;
    }

    public void Update(Vector2 movementDirection, float deltaTime, float minX, float maxX, float minY, float maxY)
    {
        // Normalize movement direction if it has magnitude
        if (movementDirection.Length() > 0)
        {
            movementDirection = Vector2.Normalize(movementDirection);
        }

        // Apply movement
        Position += movementDirection * Speed * deltaTime;

        // Clamp to bounds (accounting for radius)
        Position = new Vector2(
            Math.Clamp(Position.X, minX + Radius, maxX - Radius),
            Math.Clamp(Position.Y, minY + Radius, maxY - Radius)
        );
    }
}

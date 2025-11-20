using System;
using System.Numerics;

namespace TurretGame.Core.Entities;

public enum EnemyType
{
    Hunter,  // Chases the player
    Prey     // Flees from the player
}

public class Enemy
{
    public Vector2 Position { get; private set; }
    public float Speed { get; set; } = 160f;
    public float Health { get; set; } = 100f;
    public float Radius { get; set; } = 15f;
    public bool IsAlive { get; set; } = true;
    public EnemyType Type { get; set; }

    public Enemy(Vector2 startPosition, EnemyType type, float speed)
    {
        Position = startPosition;
        Type = type;
        Speed = speed;
    }

    public void Update(Vector2 targetPosition, float deltaTime, float minX, float maxX, float minY, float maxY)
    {
        if (!IsAlive) return;

        Vector2 direction;

        if (Type == EnemyType.Hunter)
        {
            // Hunters chase - move toward player
            direction = targetPosition - Position;
        }
        else
        {
            // Prey flee - move toward the farthest corner from player to maximize distance
            Vector2 farthestCorner = GetFarthestCorner(targetPosition, minX, maxX, minY, maxY);
            direction = farthestCorner - Position;
        }

        // Normalize and move if we have a valid direction
        if (direction.Length() > 0)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * Speed * deltaTime;
        }

        // Clamp to bounds (accounting for radius)
        Position = new Vector2(
            Math.Clamp(Position.X, minX + Radius, maxX - Radius),
            Math.Clamp(Position.Y, minY + Radius, maxY - Radius)
        );
    }

    private Vector2 GetFarthestCorner(Vector2 playerPosition, float minX, float maxX, float minY, float maxY)
    {
        // Check all four corners and find the one farthest from player
        Vector2[] corners = new[]
        {
            new Vector2(minX + Radius, minY + Radius),     // Top-left
            new Vector2(maxX - Radius, minY + Radius),     // Top-right
            new Vector2(minX + Radius, maxY - Radius),     // Bottom-left
            new Vector2(maxX - Radius, maxY - Radius)      // Bottom-right
        };

        Vector2 farthestCorner = corners[0];
        float maxDistance = Vector2.Distance(playerPosition, corners[0]);

        for (int i = 1; i < corners.Length; i++)
        {
            float distance = Vector2.Distance(playerPosition, corners[i]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestCorner = corners[i];
            }
        }

        return farthestCorner;
    }
}

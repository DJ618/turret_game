using System;
using System.Numerics;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class SpawnManager
{
    private readonly Random _random;

    public SpawnManager()
    {
        _random = new Random();
    }

    public Vector2 GetRandomEdgePosition(Bounds bounds)
    {
        int edge = _random.Next(4); // 0=top, 1=right, 2=bottom, 3=left

        return edge switch
        {
            0 => new Vector2(_random.Next((int)bounds.MinX, (int)bounds.MaxX), bounds.MinY), // Top edge
            1 => new Vector2(bounds.MaxX, _random.Next((int)bounds.MinY, (int)bounds.MaxY)), // Right edge
            2 => new Vector2(_random.Next((int)bounds.MinX, (int)bounds.MaxX), bounds.MaxY), // Bottom edge
            3 => new Vector2(bounds.MinX, _random.Next((int)bounds.MinY, (int)bounds.MaxY)), // Left edge
            _ => new Vector2(bounds.Width / 2f, bounds.MinY) // Default to top center
        };
    }

    public Enemy SpawnEnemy(EnemyType type, float speed, Bounds bounds, Vector2? playerPosition = null)
    {
        Vector2 position;

        if (type == EnemyType.Hunter && playerPosition.HasValue)
        {
            // Spawn hunters far from player - try multiple positions and pick the farthest
            position = GetFarthestEdgePosition(bounds, playerPosition.Value);
        }
        else
        {
            position = GetRandomEdgePosition(bounds);
        }

        return new Enemy(position, type, speed);
    }

    private Vector2 GetFarthestEdgePosition(Bounds bounds, Vector2 playerPosition)
    {
        // Generate multiple random edge positions and pick the one farthest from player
        Vector2 farthestPosition = GetRandomEdgePosition(bounds);
        float maxDistance = Vector2.Distance(farthestPosition, playerPosition);

        for (int i = 0; i < 5; i++) // Try 5 positions
        {
            var candidatePosition = GetRandomEdgePosition(bounds);
            float distance = Vector2.Distance(candidatePosition, playerPosition);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPosition = candidatePosition;
            }
        }

        return farthestPosition;
    }
}

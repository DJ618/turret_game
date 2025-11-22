using System;
using System.Numerics;

namespace TurretGame.Core.Entities;

public class Projectile
{
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Radius { get; set; } = 5f;
    public float Speed { get; set; } = 500f; // Pixels per second
    public bool IsActive { get; set; } = true;

    public Projectile(Vector2 startPosition, Vector2 targetPosition)
    {
        Position = startPosition;

        // Calculate direction to target
        var direction = targetPosition - startPosition;
        if (direction.Length() > 0)
        {
            direction = Vector2.Normalize(direction);
        }

        Velocity = direction * Speed;
    }

    public void Update(float deltaTime, float minX, float maxX, float minY, float maxY)
    {
        if (!IsActive) return;

        // Move projectile
        Position += Velocity * deltaTime;

        // Deactivate if out of bounds
        if (Position.X < minX || Position.X > maxX ||
            Position.Y < minY || Position.Y > maxY)
        {
            IsActive = false;
        }
    }
}

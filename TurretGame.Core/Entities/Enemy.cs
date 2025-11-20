using System.Numerics;

namespace TurretGame.Core.Entities;

public class Enemy
{
    public Vector2 Position { get; private set; }
    public float Speed { get; set; } = 160f;
    public float Health { get; set; } = 100f;
    public float Radius { get; set; } = 15f;
    public bool IsAlive { get; set; } = true;

    public Enemy(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void Update(Vector2 targetPosition, float deltaTime)
    {
        if (!IsAlive) return;

        // Calculate direction toward target
        Vector2 direction = targetPosition - Position;

        // Normalize and move if we have a valid direction
        if (direction.Length() > 0)
        {
            direction = Vector2.Normalize(direction);
            Position += direction * Speed * deltaTime;
        }
    }
}

using System.Numerics;

namespace TurretGame.Core.Entities;

public class Turret
{
    public Vector2 Position { get; private set; }
    public float Size { get; set; } = 30f; // Half-width of the square (1.5x player radius)
    public float ShootCooldown { get; set; } = 2.0f; // Seconds between shots
    public float TimeSinceLastShot { get; set; } = 0f;

    public Turret(Vector2 position)
    {
        Position = position;
    }

    public void Update(float deltaTime)
    {
        TimeSinceLastShot += deltaTime;
    }

    public bool CanShoot()
    {
        return TimeSinceLastShot >= ShootCooldown;
    }

    public void ResetShootCooldown()
    {
        TimeSinceLastShot = 0f;
    }
}

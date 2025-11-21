using System.Numerics;

namespace TurretGame.Core.Entities;

public class ResourcePickup
{
    public Vector2 Position { get; private set; }
    public float Radius { get; set; } = 8f;
    public int Value { get; set; } = 1;
    public bool IsCollected { get; set; } = false;

    public ResourcePickup(Vector2 position, int value = 1)
    {
        Position = position;
        Value = value;
    }
}

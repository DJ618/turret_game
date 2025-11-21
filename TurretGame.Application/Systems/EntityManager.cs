using System.Collections.Generic;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class EntityManager
{
    private readonly List<Enemy> _enemies;
    private readonly List<ResourcePickup> _pickups;

    public EntityManager()
    {
        _enemies = new List<Enemy>();
        _pickups = new List<ResourcePickup>();
    }

    public IReadOnlyList<Enemy> Enemies => _enemies;
    public IReadOnlyList<ResourcePickup> Pickups => _pickups;

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void AddPickup(ResourcePickup pickup)
    {
        _pickups.Add(pickup);
    }

    public void RemoveCollectedPickups()
    {
        _pickups.RemoveAll(p => p.IsCollected);
    }

    public void UpdateEnemies(System.Numerics.Vector2 playerPosition, float deltaTime, Bounds bounds)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Update(playerPosition, deltaTime, bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);
        }

        // Apply collision avoidance for hunters
        ResolveHunterCollisions();
    }

    private void ResolveHunterCollisions()
    {
        // Check all pairs of hunters for collision
        for (int i = 0; i < _enemies.Count; i++)
        {
            var enemy1 = _enemies[i];
            if (!enemy1.IsAlive || enemy1.Type != EnemyType.Hunter) continue;

            for (int j = i + 1; j < _enemies.Count; j++)
            {
                var enemy2 = _enemies[j];
                if (!enemy2.IsAlive || enemy2.Type != EnemyType.Hunter) continue;

                // Calculate distance between hunters
                var direction = enemy2.Position - enemy1.Position;
                float distance = direction.Length();

                // Allow 1/4 overlap: min distance = (r1 + r2) * 0.75
                float minDistance = (enemy1.Radius + enemy2.Radius) * 0.75f;

                if (distance < minDistance && distance > 0)
                {
                    // Too much overlap - push them apart
                    var pushDirection = System.Numerics.Vector2.Normalize(direction);
                    float overlap = minDistance - distance;
                    float pushAmount = overlap * 0.5f; // Each moves half the overlap distance

                    // Push enemies apart
                    enemy1.SetPosition(enemy1.Position - pushDirection * pushAmount);
                    enemy2.SetPosition(enemy2.Position + pushDirection * pushAmount);
                }
            }
        }
    }

    public void Clear()
    {
        _enemies.Clear();
        _pickups.Clear();
    }
}

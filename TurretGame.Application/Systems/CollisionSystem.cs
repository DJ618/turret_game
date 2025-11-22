using System;
using System.Linq;
using TurretGame.Core.Entities;
using TurretGame.Core.Interfaces;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class CollisionSystem
{
    private readonly ICollisionHandler _collisionHandler;

    public CollisionSystem(ICollisionHandler collisionHandler)
    {
        _collisionHandler = collisionHandler;
    }

    public void CheckCollisions(Player player, EntityManager entityManager, Bounds bounds)
    {
        // Check player-enemy collisions
        foreach (var enemy in entityManager.Enemies)
        {
            if (enemy.IsAlive && CollisionDetection.CircleToCircle(
                player.Position, player.Radius,
                enemy.Position, enemy.Radius))
            {
                _collisionHandler.OnPlayerEnemyCollision(player, enemy, bounds);
            }
        }

        // Check player-pickup collisions
        foreach (var pickup in entityManager.Pickups)
        {
            if (!pickup.IsCollected && CollisionDetection.CircleToCircle(
                player.Position, player.Radius,
                pickup.Position, pickup.Radius))
            {
                _collisionHandler.OnPlayerPickupCollision(player, pickup);
            }
        }

        // Check enemy-enemy collisions (prevent overlapping beyond half their width)
        var enemies = entityManager.Enemies.ToList();
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = i + 1; j < enemies.Count; j++)
            {
                var enemy1 = enemies[i];
                var enemy2 = enemies[j];

                if (!enemy1.IsAlive || !enemy2.IsAlive) continue;

                var distance = System.Numerics.Vector2.Distance(enemy1.Position, enemy2.Position);
                var minDistance = (enemy1.Radius + enemy2.Radius) * 0.5f; // Allow overlap of half their combined width

                if (distance < minDistance && distance > 0.001f)
                {
                    // Push enemies apart
                    var direction = System.Numerics.Vector2.Normalize(enemy2.Position - enemy1.Position);
                    var pushAmount = (minDistance - distance) * 0.5f;

                    var newPos1 = enemy1.Position - direction * pushAmount;
                    var newPos2 = enemy2.Position + direction * pushAmount;

                    // Clamp positions to bounds
                    enemy1.SetPosition(new System.Numerics.Vector2(
                        Math.Clamp(newPos1.X, bounds.MinX + enemy1.Radius, bounds.MaxX - enemy1.Radius),
                        Math.Clamp(newPos1.Y, bounds.MinY + enemy1.Radius, bounds.MaxY - enemy1.Radius)
                    ));
                    enemy2.SetPosition(new System.Numerics.Vector2(
                        Math.Clamp(newPos2.X, bounds.MinX + enemy2.Radius, bounds.MaxX - enemy2.Radius),
                        Math.Clamp(newPos2.Y, bounds.MinY + enemy2.Radius, bounds.MaxY - enemy2.Radius)
                    ));
                }
            }
        }
    }
}

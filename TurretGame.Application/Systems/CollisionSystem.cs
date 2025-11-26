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
        // Check player-turret collisions (player cannot pass through turrets)
        foreach (var turret in entityManager.Turrets)
        {
            if (CollisionDetection.CircleToSquare(
                player.Position, player.Radius,
                turret.Position, turret.Size))
            {
                // Push player away from turret
                var direction = player.Position - turret.Position;
                if (direction.Length() > 0)
                {
                    direction = System.Numerics.Vector2.Normalize(direction);

                    // Calculate how far to push the player
                    // Find closest point on square to player, then ensure player is at least their radius away
                    float closestX = Math.Clamp(player.Position.X, turret.Position.X - turret.Size, turret.Position.X + turret.Size);
                    float closestY = Math.Clamp(player.Position.Y, turret.Position.Y - turret.Size, turret.Position.Y + turret.Size);
                    var closestPoint = new System.Numerics.Vector2(closestX, closestY);

                    var pushDirection = player.Position - closestPoint;
                    if (pushDirection.Length() > 0)
                    {
                        pushDirection = System.Numerics.Vector2.Normalize(pushDirection);
                        player.SetPosition(closestPoint + pushDirection * player.Radius);
                    }
                }
            }
        }

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

        // Check enemy-turret collisions (enemies cannot pass through turrets)
        foreach (var enemy in enemies)
        {
            if (!enemy.IsAlive) continue;

            foreach (var turret in entityManager.Turrets)
            {
                if (CollisionDetection.CircleToSquare(
                    enemy.Position, enemy.Radius,
                    turret.Position, turret.Size))
                {
                    // Push enemy away from turret
                    float closestX = Math.Clamp(enemy.Position.X, turret.Position.X - turret.Size, turret.Position.X + turret.Size);
                    float closestY = Math.Clamp(enemy.Position.Y, turret.Position.Y - turret.Size, turret.Position.Y + turret.Size);
                    var closestPoint = new System.Numerics.Vector2(closestX, closestY);

                    var pushDirection = enemy.Position - closestPoint;
                    if (pushDirection.Length() > 0)
                    {
                        pushDirection = System.Numerics.Vector2.Normalize(pushDirection);
                        var newPosition = closestPoint + pushDirection * enemy.Radius;

                        // Clamp to bounds
                        enemy.SetPosition(new System.Numerics.Vector2(
                            Math.Clamp(newPosition.X, bounds.MinX + enemy.Radius, bounds.MaxX - enemy.Radius),
                            Math.Clamp(newPosition.Y, bounds.MinY + enemy.Radius, bounds.MaxY - enemy.Radius)
                        ));
                    }
                }
            }
        }
    }
}

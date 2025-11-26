using System;
using System.Numerics;
using TurretGame.Application.State;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class ProjectileSystem
{
    private readonly EntityManager _entityManager;
    private readonly StatisticsManager _statisticsManager;
    private readonly Random _random;

    public ProjectileSystem(EntityManager entityManager, StatisticsManager statisticsManager)
    {
        _entityManager = entityManager;
        _statisticsManager = statisticsManager;
        _random = new Random();
    }

    public void Update(float deltaTime, Bounds bounds)
    {
        foreach (var projectile in _entityManager.Projectiles)
        {
            if (!projectile.IsActive) continue;

            // Update projectile position
            projectile.Update(deltaTime, bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);

            // Check collision with turrets (projectiles are destroyed by turrets)
            bool hitTurret = false;
            foreach (var turret in _entityManager.Turrets)
            {
                if (CollisionDetection.CircleToSquare(
                    projectile.Position, projectile.Radius,
                    turret.Position, turret.Size))
                {
                    // Hit turret - destroy projectile
                    projectile.IsActive = false;
                    hitTurret = true;
                    break;
                }
            }

            if (hitTurret) continue;

            // Check collision with enemies
            foreach (var enemy in _entityManager.Enemies)
            {
                if (!enemy.IsAlive) continue;

                if (CollisionDetection.CircleToCircle(
                    projectile.Position, projectile.Radius,
                    enemy.Position, enemy.Radius))
                {
                    // Hit enemy - kill it
                    enemy.IsAlive = false;
                    projectile.IsActive = false;
                    _statisticsManager.IncrementEnemiesKilled();

                    // Spawn resource pickup, avoiding turrets
                    const float pickupRadius = 8f;
                    Vector2 pickupPosition = enemy.Position;

                    // Check if enemy position overlaps with turret
                    if (IsPositionOverlappingTurret(enemy.Position, pickupRadius))
                    {
                        // Try to find a valid position nearby
                        bool foundValidPosition = false;
                        for (int attempt = 0; attempt < 10; attempt++)
                        {
                            var offset = GetRandomOffset(pickupRadius * 2, pickupRadius * 4);
                            var testPosition = enemy.Position + offset;

                            // Clamp to bounds
                            testPosition = new Vector2(
                                Math.Clamp(testPosition.X, bounds.MinX + pickupRadius, bounds.MaxX - pickupRadius),
                                Math.Clamp(testPosition.Y, bounds.MinY + pickupRadius, bounds.MaxY - pickupRadius)
                            );

                            if (!IsPositionOverlappingTurret(testPosition, pickupRadius))
                            {
                                pickupPosition = testPosition;
                                foundValidPosition = true;
                                break;
                            }
                        }

                        // If still overlapping after attempts, offset slightly from enemy position
                        if (!foundValidPosition)
                        {
                            pickupPosition = enemy.Position + new Vector2(pickupRadius * 2, pickupRadius * 2);
                        }
                    }

                    var pickup = new ResourcePickup(pickupPosition, 1);
                    _entityManager.AddPickup(pickup);

                    break; // Projectile can only hit one enemy
                }
            }
        }

        // Remove inactive projectiles
        _entityManager.RemoveInactiveProjectiles();
    }

    private bool IsPositionOverlappingTurret(Vector2 position, float radius)
    {
        foreach (var turret in _entityManager.Turrets)
        {
            if (CollisionDetection.CircleToSquare(position, radius, turret.Position, turret.Size))
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetRandomOffset(float minDistance, float maxDistance)
    {
        // Generate random angle
        float angle = (float)(_random.NextDouble() * 2 * Math.PI);

        // Generate random distance
        float distance = minDistance + (float)(_random.NextDouble() * (maxDistance - minDistance));

        // Convert polar to cartesian coordinates
        return new Vector2(
            (float)Math.Cos(angle) * distance,
            (float)Math.Sin(angle) * distance
        );
    }
}

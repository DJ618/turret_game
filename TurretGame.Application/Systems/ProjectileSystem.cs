using TurretGame.Application.State;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class ProjectileSystem
{
    private readonly EntityManager _entityManager;
    private readonly StatisticsManager _statisticsManager;

    public ProjectileSystem(EntityManager entityManager, StatisticsManager statisticsManager)
    {
        _entityManager = entityManager;
        _statisticsManager = statisticsManager;
    }

    public void Update(float deltaTime, Bounds bounds)
    {
        foreach (var projectile in _entityManager.Projectiles)
        {
            if (!projectile.IsActive) continue;

            // Update projectile position
            projectile.Update(deltaTime, bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);

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

                    // Spawn resource pickup at enemy position (not projectile position)
                    var pickup = new ResourcePickup(enemy.Position, 1);
                    _entityManager.AddPickup(pickup);

                    break; // Projectile can only hit one enemy
                }
            }
        }

        // Remove inactive projectiles
        _entityManager.RemoveInactiveProjectiles();
    }
}

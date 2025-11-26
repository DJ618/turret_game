using System;
using System.Linq;
using System.Numerics;
using TurretGame.Core.Entities;

namespace TurretGame.Application.Systems;

public class TurretSystem
{
    private readonly EntityManager _entityManager;
    private readonly UpgradeManager _upgradeManager;

    public TurretSystem(EntityManager entityManager, UpgradeManager upgradeManager)
    {
        _entityManager = entityManager;
        _upgradeManager = upgradeManager;
    }

    public void Update(float deltaTime)
    {
        foreach (var turret in _entityManager.Turrets)
        {
            turret.Update(deltaTime);

            if (turret.CanShoot())
            {
                var nearestEnemy = FindNearestEnemy(turret.Position);
                if (nearestEnemy != null)
                {
                    // Calculate direction to enemy
                    var direction = Vector2.Normalize(nearestEnemy.Position - turret.Position);

                    // Spawn projectile outside the turret's bounding box
                    // Use diagonal distance (Size * sqrt(2)) plus a small buffer to ensure it's outside
                    float spawnDistance = turret.Size * 1.5f; // 1.414 (sqrt(2)) rounded up for safety
                    var spawnPosition = turret.Position + direction * spawnDistance;

                    // Create projectile aimed at nearest enemy with velocity upgrade applied
                    var projectile = new Projectile(spawnPosition, nearestEnemy.Position);
                    projectile.Speed *= _upgradeManager.UpgradeState.ProjectileVelocityMultiplier;
                    _entityManager.AddProjectile(projectile);
                    turret.ResetShootCooldown();
                }
            }
        }
    }

    private Enemy FindNearestEnemy(Vector2 turretPosition)
    {
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var enemy in _entityManager.Enemies)
        {
            if (!enemy.IsAlive) continue;

            float distance = Vector2.Distance(turretPosition, enemy.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}

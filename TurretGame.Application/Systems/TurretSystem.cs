using System;
using System.Linq;
using System.Numerics;
using TurretGame.Core.Entities;

namespace TurretGame.Application.Systems;

public class TurretSystem
{
    private readonly EntityManager _entityManager;

    public TurretSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
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
                    // Create projectile aimed at nearest enemy
                    var projectile = new Projectile(turret.Position, nearestEnemy.Position);
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

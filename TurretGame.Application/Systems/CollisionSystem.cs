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

    public void CheckCollisions(Player player, EntityManager entityManager)
    {
        // Check player-enemy collisions
        foreach (var enemy in entityManager.Enemies)
        {
            if (enemy.IsAlive && CollisionDetection.CircleToCircle(
                player.Position, player.Radius,
                enemy.Position, enemy.Radius))
            {
                _collisionHandler.OnPlayerEnemyCollision(player, enemy);
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
    }
}

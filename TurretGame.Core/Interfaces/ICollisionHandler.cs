using TurretGame.Core.Entities;

namespace TurretGame.Core.Interfaces;

public interface ICollisionHandler
{
    void OnPlayerEnemyCollision(Player player, Enemy enemy);
    void OnPlayerPickupCollision(Player player, ResourcePickup pickup);
}

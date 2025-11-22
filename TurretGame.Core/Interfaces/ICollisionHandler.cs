using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Core.Interfaces;

public interface ICollisionHandler
{
    void OnPlayerEnemyCollision(Player player, Enemy enemy, Bounds bounds);
    void OnPlayerPickupCollision(Player player, ResourcePickup pickup);
}

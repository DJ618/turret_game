using System;
using System.Numerics;
using TurretGame.Application.State;
using TurretGame.Application.Systems;
using TurretGame.Core.Entities;
using TurretGame.Core.Interfaces;
using TurretGame.Core.State;

namespace TurretGame.Application;

public class GameCollisionHandler : ICollisionHandler
{
    private readonly GameStateManager _gameStateManager;
    private readonly ResourceManager _resourceManager;
    private readonly EntityManager _entityManager;
    private readonly StatisticsManager _statisticsManager;
    private readonly Random _random;

    public GameCollisionHandler(
        GameStateManager gameStateManager,
        ResourceManager resourceManager,
        EntityManager entityManager,
        StatisticsManager statisticsManager)
    {
        _gameStateManager = gameStateManager;
        _resourceManager = resourceManager;
        _entityManager = entityManager;
        _statisticsManager = statisticsManager;
        _random = new Random();
    }

    public void OnPlayerEnemyCollision(Player player, Enemy enemy)
    {
        if (enemy.Type == EnemyType.Hunter)
        {
            // Collision with hunter = game over
            _gameStateManager.ChangeState(GameState.GameOver);
        }
        else
        {
            // Collision with prey = prey dies and drops resource
            enemy.IsAlive = false;
            _statisticsManager.IncrementEnemiesKilled();

            // Spawn resource pickup with random offset from enemy's position
            var offset = GetRandomOffset(80f, 150f); // Random distance between 80-150 pixels
            var pickupPosition = enemy.Position + offset;
            var pickup = new ResourcePickup(pickupPosition, 1);
            _entityManager.AddPickup(pickup);
        }
    }

    public void OnPlayerPickupCollision(Player player, ResourcePickup pickup)
    {
        // Collect the resource
        pickup.IsCollected = true;
        _resourceManager.CollectResource(pickup.Value);
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

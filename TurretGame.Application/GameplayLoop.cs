using TurretGame.Application.State;
using TurretGame.Application.Systems;
using TurretGame.Core.Entities;
using TurretGame.Core.Interfaces;
using TurretGame.Core.Utilities;

namespace TurretGame.Application;

public class GameplayLoop
{
    private readonly Player _player;
    private readonly EntityManager _entityManager;
    private readonly CollisionSystem _collisionSystem;
    private readonly GameStateManager _gameStateManager;
    private readonly ResourceManager _resourceManager;
    private readonly WaveManager _waveManager;
    private readonly IInputService _inputService;

    public GameplayLoop(
        Player player,
        EntityManager entityManager,
        CollisionSystem collisionSystem,
        GameStateManager gameStateManager,
        ResourceManager resourceManager,
        WaveManager waveManager,
        IInputService inputService)
    {
        _player = player;
        _entityManager = entityManager;
        _collisionSystem = collisionSystem;
        _gameStateManager = gameStateManager;
        _resourceManager = resourceManager;
        _waveManager = waveManager;
        _inputService = inputService;
    }

    public Player Player => _player;
    public EntityManager EntityManager => _entityManager;
    public GameStateManager GameStateManager => _gameStateManager;
    public ResourceManager ResourceManager => _resourceManager;
    public WaveManager WaveManager => _waveManager;

    public bool ShouldExit()
    {
        return _inputService.IsExitRequested();
    }

    public void Update(float deltaTime, Bounds bounds)
    {
        // Don't update if game is over
        if (_gameStateManager.IsGameOver())
        {
            return;
        }

        // Check if we should start the next wave
        if (_waveManager.ShouldStartNextWave())
        {
            _waveManager.StartNextWave(bounds, _player.Position);
        }

        // Get input and update player
        var movementDirection = _inputService.GetMovementDirection();
        _player.Update(movementDirection, deltaTime, bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);

        // Update enemies
        _entityManager.UpdateEnemies(_player.Position, deltaTime, bounds);

        // Check collisions
        _collisionSystem.CheckCollisions(_player, _entityManager);

        // Remove collected pickups
        _entityManager.RemoveCollectedPickups();
    }
}

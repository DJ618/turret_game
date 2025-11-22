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
    private readonly TurretSystem _turretSystem;
    private readonly ProjectileSystem _projectileSystem;
    private readonly StatisticsManager _statisticsManager;
    private readonly IInputService _inputService;

    private bool _turretPlacedThisRound;

    public GameplayLoop(
        Player player,
        EntityManager entityManager,
        CollisionSystem collisionSystem,
        GameStateManager gameStateManager,
        ResourceManager resourceManager,
        WaveManager waveManager,
        TurretSystem turretSystem,
        ProjectileSystem projectileSystem,
        StatisticsManager statisticsManager,
        IInputService inputService)
    {
        _player = player;
        _entityManager = entityManager;
        _collisionSystem = collisionSystem;
        _gameStateManager = gameStateManager;
        _resourceManager = resourceManager;
        _waveManager = waveManager;
        _turretSystem = turretSystem;
        _projectileSystem = projectileSystem;
        _statisticsManager = statisticsManager;
        _inputService = inputService;
    }

    public Player Player => _player;
    public EntityManager EntityManager => _entityManager;
    public GameStateManager GameStateManager => _gameStateManager;
    public ResourceManager ResourceManager => _resourceManager;
    public WaveManager WaveManager => _waveManager;
    public bool TurretPlacedThisRound => _turretPlacedThisRound;

    public bool ShouldExit()
    {
        return _inputService.IsExitRequested();
    }

    public void Update(float deltaTime, Bounds bounds)
    {
        // Don't update if game is over or placing turret
        if (_gameStateManager.IsGameOver() || _gameStateManager.IsPlacingTurret())
        {
            return;
        }

        // Check if we should transition to turret placement or start next wave
        if (_waveManager.ShouldStartNextWave())
        {
            // Wave 1 starts immediately without turret placement
            if (_waveManager.CurrentWave == 0)
            {
                _waveManager.StartNextWave(bounds, _player.Position);
                return;
            }
            // After wave 1, allow turret placement
            else
            {
                _gameStateManager.ChangeState(Core.State.GameState.PlacingTurret);
                _turretPlacedThisRound = false; // Reset for new placement phase
                return;
            }
        }

        // Get input and update player
        var movementDirection = _inputService.GetMovementDirection();
        _player.Update(movementDirection, deltaTime, bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);

        // Update enemies
        _entityManager.UpdateEnemies(_player.Position, deltaTime, bounds);

        // Update turrets and projectiles
        _turretSystem.Update(deltaTime);
        _projectileSystem.Update(deltaTime, bounds);

        // Check collisions
        _collisionSystem.CheckCollisions(_player, _entityManager, bounds);

        // Remove collected pickups
        _entityManager.RemoveCollectedPickups();
    }

    public void PlaceTurret(System.Numerics.Vector2 position)
    {
        // Only place if in PlacingTurret state and haven't placed yet
        if (!_gameStateManager.IsPlacingTurret() || _turretPlacedThisRound)
        {
            return;
        }

        var turret = new Turret(position);
        _entityManager.AddTurret(turret);
        _turretPlacedThisRound = true;
    }

    public void ContinueToNextWave(Bounds bounds)
    {
        _waveManager.StartNextWave(bounds, _player.Position);
        _gameStateManager.ChangeState(Core.State.GameState.Playing);
    }

    public void Restart(Bounds bounds, System.Numerics.Vector2 screenCenter)
    {
        // Reset player position to center
        _player.SetPosition(screenCenter);

        // Clear all entities
        _entityManager.Clear();

        // Reset all managers
        _gameStateManager.Reset();
        _resourceManager.Reset();
        _statisticsManager.Reset();
        _waveManager.Reset();

        // Reset turret placement flag
        _turretPlacedThisRound = false;

        // Start wave 1
        _waveManager.StartNextWave(bounds, _player.Position);
    }
}

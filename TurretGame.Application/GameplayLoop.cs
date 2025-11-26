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
    private readonly UpgradeManager _upgradeManager;
    private readonly IInputService _inputService;

    private int _turretsPlacedThisRound;
    private int _turretsAllowedThisRound;

    private const int MAX_TURRETS = 15; // Maximum turrets allowed on the board

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
        UpgradeManager upgradeManager,
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
        _upgradeManager = upgradeManager;
        _inputService = inputService;
        _turretsAllowedThisRound = 1; // Base is 1 turret per round
    }

    public Player Player => _player;
    public EntityManager EntityManager => _entityManager;
    public GameStateManager GameStateManager => _gameStateManager;
    public ResourceManager ResourceManager => _resourceManager;
    public WaveManager WaveManager => _waveManager;
    public UpgradeManager UpgradeManager => _upgradeManager;
    public int TurretsPlacedThisRound => _turretsPlacedThisRound;
    public int TurretsAllowedThisRound => _turretsAllowedThisRound;

    public bool ShouldExit()
    {
        return _inputService.IsExitRequested();
    }

    public void Update(float deltaTime, Bounds bounds)
    {
        // Don't update if game is over, placing turret, or choosing upgrade
        if (_gameStateManager.IsGameOver() || _gameStateManager.IsPlacingTurret() || _gameStateManager.IsChoosingUpgrade())
        {
            return;
        }

        // Check if we should transition to upgrade selection or start next wave
        if (_waveManager.ShouldStartNextWave())
        {
            // Wave 1 starts immediately without upgrades
            if (_waveManager.CurrentWave == 0)
            {
                _waveManager.StartNextWave(bounds, _player.Position);
                return;
            }
            // After wave 1, show upgrade selection
            else
            {
                _gameStateManager.ChangeState(Core.State.GameState.ChoosingUpgrade);
                _upgradeManager.GenerateUpgradeOptions(_entityManager.Turrets.Count, MAX_TURRETS);
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

    public void SelectUpgrade(int optionIndex, Bounds bounds)
    {
        // Only select if in ChoosingUpgrade state
        if (!_gameStateManager.IsChoosingUpgrade())
        {
            return;
        }

        _upgradeManager.SelectUpgrade(optionIndex);

        // Check if we've reached max turrets - if so, skip turret placement
        int currentTurretCount = _entityManager.Turrets.Count;
        if (currentTurretCount >= MAX_TURRETS)
        {
            // Skip turret placement, go straight to next wave
            _gameStateManager.ChangeState(Core.State.GameState.Playing);
            _waveManager.StartNextWave(bounds, _player.Position);
            return;
        }

        // Calculate how many turrets we can place this round
        int remainingTurretSlots = MAX_TURRETS - currentTurretCount;
        int desiredTurrets = 1 + _upgradeManager.UpgradeState.ExtraTurretsPerRound;
        _turretsAllowedThisRound = System.Math.Min(desiredTurrets, remainingTurretSlots);

        // Transition to turret placement
        _gameStateManager.ChangeState(Core.State.GameState.PlacingTurret);
        _turretsPlacedThisRound = 0;
    }

    public void PlaceTurret(System.Numerics.Vector2 position)
    {
        // Only place if in PlacingTurret state and haven't placed max turrets yet
        if (!_gameStateManager.IsPlacingTurret() || _turretsPlacedThisRound >= _turretsAllowedThisRound)
        {
            return;
        }

        var turret = new Turret(position);
        _entityManager.AddTurret(turret);
        _turretsPlacedThisRound++;
    }

    public bool CanPlaceMoreTurrets()
    {
        return _turretsPlacedThisRound < _turretsAllowedThisRound;
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
        _upgradeManager.Reset();

        // Reset turret placement counters
        _turretsPlacedThisRound = 0;
        _turretsAllowedThisRound = 1;

        // Start wave 1
        _waveManager.StartNextWave(bounds, _player.Position);
    }
}

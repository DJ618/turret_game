using System.Numerics;
using Microsoft.Extensions.Options;
using TurretGame.Core.Configuration;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class WaveManager
{
    private readonly SpawnManager _spawnManager;
    private readonly EntityManager _entityManager;
    private readonly UpgradeManager _upgradeManager;
    private readonly IOptionsMonitor<GameSettings> _gameSettings;

    private int _currentWave;
    private int _previousFibonacci;
    private int _currentFibonacci;
    private float _enemySpeedMultiplier;

    public int CurrentWave => _currentWave;

    public WaveManager(SpawnManager spawnManager, EntityManager entityManager, UpgradeManager upgradeManager, IOptionsMonitor<GameSettings> gameSettings)
    {
        _spawnManager = spawnManager;
        _entityManager = entityManager;
        _upgradeManager = upgradeManager;
        _gameSettings = gameSettings;
        _currentWave = 0;
        _previousFibonacci = 1;
        _currentFibonacci = 1;
        _enemySpeedMultiplier = 1.0f; // Start at 100%
    }

    public void StartNextWave(Bounds bounds, Vector2 playerPosition)
    {
        _currentWave++;

        // Get current settings
        var settings = _gameSettings.CurrentValue;

        // Increase enemy speed by 1% each round (after wave 1)
        if (_currentWave > 1)
        {
            _enemySpeedMultiplier *= 1.01f;

            // Cap at 90% of player speed
            float maxSpeedMultiplier = (settings.PlayerSpeed * 0.9f) / settings.PreySpeed; // Calculate max based on prey base speed
            _enemySpeedMultiplier = Math.Min(_enemySpeedMultiplier, maxSpeedMultiplier);
        }

        // Prey count follows Fibonacci sequence
        int preyCount = _currentFibonacci;

        // Calculate next Fibonacci number for next wave
        int nextFibonacci = _previousFibonacci + _currentFibonacci;
        _previousFibonacci = _currentFibonacci;
        _currentFibonacci = nextFibonacci;

        // Hunter count increases based on settings (multiplied by wave number)
        int hunterCount = _currentWave * settings.HuntersPerWave;

        // Spawn prey enemies using configured speed with upgrade multipliers and wave speed increase applied
        float adjustedPreySpeed = settings.PreySpeed * _upgradeManager.UpgradeState.PreySpeedMultiplier * _enemySpeedMultiplier;
        for (int i = 0; i < preyCount; i++)
        {
            var prey = _spawnManager.SpawnEnemy(EnemyType.Prey, adjustedPreySpeed, bounds);
            _entityManager.AddEnemy(prey);
        }

        // Spawn hunter enemies using configured speed with upgrade multipliers and wave speed increase applied (spawned far from player)
        float adjustedHunterSpeed = settings.HunterSpeed * _upgradeManager.UpgradeState.HunterSpeedMultiplier * _enemySpeedMultiplier;
        for (int i = 0; i < hunterCount; i++)
        {
            var hunter = _spawnManager.SpawnEnemy(EnemyType.Hunter, adjustedHunterSpeed, bounds, playerPosition);
            _entityManager.AddEnemy(hunter);
        }
    }

    public bool ShouldStartNextWave()
    {
        // Start next wave if all PREY enemies are dead (hunters don't count)
        foreach (var enemy in _entityManager.Enemies)
        {
            if (enemy.IsAlive && enemy.Type == EnemyType.Prey)
            {
                return false;
            }
        }
        return true;
    }

    public void Reset()
    {
        _currentWave = 0;
        _previousFibonacci = 1;
        _currentFibonacci = 1;
        _enemySpeedMultiplier = 1.0f; // Reset speed to 100%
    }
}

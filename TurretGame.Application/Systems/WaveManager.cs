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
    private readonly IOptionsMonitor<GameSettings> _gameSettings;

    private int _currentWave;
    private int _previousFibonacci;
    private int _currentFibonacci;

    public int CurrentWave => _currentWave;

    public WaveManager(SpawnManager spawnManager, EntityManager entityManager, IOptionsMonitor<GameSettings> gameSettings)
    {
        _spawnManager = spawnManager;
        _entityManager = entityManager;
        _gameSettings = gameSettings;
        _currentWave = 0;
        _previousFibonacci = 1;
        _currentFibonacci = 1;
    }

    public void StartNextWave(Bounds bounds, Vector2 playerPosition)
    {
        _currentWave++;

        // Get current settings
        var settings = _gameSettings.CurrentValue;

        // Prey count follows Fibonacci sequence
        int preyCount = _currentFibonacci;

        // Calculate next Fibonacci number for next wave
        int nextFibonacci = _previousFibonacci + _currentFibonacci;
        _previousFibonacci = _currentFibonacci;
        _currentFibonacci = nextFibonacci;

        // Hunter count increases based on settings (multiplied by wave number)
        int hunterCount = _currentWave * settings.HuntersPerWave;

        // Spawn prey enemies using configured speed
        for (int i = 0; i < preyCount; i++)
        {
            var prey = _spawnManager.SpawnEnemy(EnemyType.Prey, settings.PreySpeed, bounds);
            _entityManager.AddEnemy(prey);
        }

        // Spawn hunter enemies using configured speed (spawned far from player)
        for (int i = 0; i < hunterCount; i++)
        {
            var hunter = _spawnManager.SpawnEnemy(EnemyType.Hunter, settings.HunterSpeed, bounds, playerPosition);
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
    }
}

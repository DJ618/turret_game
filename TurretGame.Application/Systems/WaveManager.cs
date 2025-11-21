using System.Numerics;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;

namespace TurretGame.Application.Systems;

public class WaveManager
{
    private readonly SpawnManager _spawnManager;
    private readonly EntityManager _entityManager;

    private int _currentWave;
    private int _previousFibonacci;
    private int _currentFibonacci;

    public int CurrentWave => _currentWave;

    public WaveManager(SpawnManager spawnManager, EntityManager entityManager)
    {
        _spawnManager = spawnManager;
        _entityManager = entityManager;
        _currentWave = 0;
        _previousFibonacci = 1;
        _currentFibonacci = 1;
    }

    public void StartNextWave(Bounds bounds, Vector2 playerPosition)
    {
        _currentWave++;

        // Prey count follows Fibonacci sequence
        int preyCount = _currentFibonacci;

        // Calculate next Fibonacci number for next wave
        int nextFibonacci = _previousFibonacci + _currentFibonacci;
        _previousFibonacci = _currentFibonacci;
        _currentFibonacci = nextFibonacci;

        // Hunter count increases linearly (1 per wave)
        int hunterCount = _currentWave;

        // Spawn prey enemies (80% player speed = 320f)
        for (int i = 0; i < preyCount; i++)
        {
            var prey = _spawnManager.SpawnEnemy(EnemyType.Prey, 320f, bounds);
            _entityManager.AddEnemy(prey);
        }

        // Spawn hunter enemies (45% player speed = 180f, spawned far from player)
        for (int i = 0; i < hunterCount; i++)
        {
            var hunter = _spawnManager.SpawnEnemy(EnemyType.Hunter, 180f, bounds, playerPosition);
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
}

namespace TurretGame.Application.State;

public class StatisticsManager
{
    public int EnemiesKilled { get; private set; } = 0;

    public void IncrementEnemiesKilled()
    {
        EnemiesKilled++;
    }

    public int CalculateScore(int highestLevelReached, int resourcesCollected)
    {
        // score = ((highest level reached) * (resources collected)) + enemies killed
        return (highestLevelReached * resourcesCollected) + EnemiesKilled;
    }

    public void Reset()
    {
        EnemiesKilled = 0;
    }
}

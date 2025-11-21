namespace TurretGame.Application.State;

public class StatisticsManager
{
    public int EnemiesKilled { get; private set; } = 0;

    public void IncrementEnemiesKilled()
    {
        EnemiesKilled++;
    }

    public void Reset()
    {
        EnemiesKilled = 0;
    }
}

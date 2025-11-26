namespace TurretGame.Core.Upgrades;

public class SlowPreyUpgrade : IUpgrade
{
    public string Name => "Slow Fleeing Enemies";
    public string Description => "-5% fleeing enemy speed";

    public void Apply(UpgradeState state)
    {
        state.PreySpeedMultiplier *= 0.95f;
    }
}

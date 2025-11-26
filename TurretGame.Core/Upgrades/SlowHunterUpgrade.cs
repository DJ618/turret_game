namespace TurretGame.Core.Upgrades;

public class SlowHunterUpgrade : IUpgrade
{
    public string Name => "Slow Chase Enemies";
    public string Description => "-5% chase enemy speed";

    public void Apply(UpgradeState state)
    {
        state.HunterSpeedMultiplier *= 0.95f;
    }
}

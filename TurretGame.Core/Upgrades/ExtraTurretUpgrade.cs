namespace TurretGame.Core.Upgrades;

public class ExtraTurretUpgrade : IUpgrade
{
    public string Name => "Extra Turret";
    public string Description => "+1 turret placement per round";

    public void Apply(UpgradeState state)
    {
        state.ExtraTurretsPerRound++;
    }
}

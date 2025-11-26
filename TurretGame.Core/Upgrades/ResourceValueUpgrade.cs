namespace TurretGame.Core.Upgrades;

public class ResourceValueUpgrade : IUpgrade
{
    public string Name => "Better Resources";
    public string Description => "+1 resource value per pickup";

    public void Apply(UpgradeState state)
    {
        state.ResourceValueBonus++;
    }
}

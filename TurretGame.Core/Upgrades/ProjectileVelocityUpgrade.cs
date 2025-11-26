namespace TurretGame.Core.Upgrades;

public class ProjectileVelocityUpgrade : IUpgrade
{
    public string Name => "Faster Projectiles";
    public string Description => "+25% projectile velocity";

    public void Apply(UpgradeState state)
    {
        state.ProjectileVelocityMultiplier *= 1.25f;
    }
}

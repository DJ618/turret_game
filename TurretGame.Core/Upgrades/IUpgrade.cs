namespace TurretGame.Core.Upgrades;

public interface IUpgrade
{
    string Name { get; }
    string Description { get; }
    void Apply(UpgradeState state);
}

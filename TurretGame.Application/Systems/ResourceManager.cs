using TurretGame.Core.Upgrades;

namespace TurretGame.Application.Systems;

public class ResourceManager
{
    private readonly UpgradeManager _upgradeManager;
    public int ResourceCount { get; private set; }

    public ResourceManager(UpgradeManager upgradeManager)
    {
        _upgradeManager = upgradeManager;
        ResourceCount = 0;
    }

    public void CollectResource(int baseAmount = 1)
    {
        int actualAmount = baseAmount * _upgradeManager.UpgradeState.ResourceValue;
        ResourceCount += actualAmount;
    }

    public bool TrySpendResource(int amount)
    {
        if (ResourceCount >= amount)
        {
            ResourceCount -= amount;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        ResourceCount = 0;
    }
}

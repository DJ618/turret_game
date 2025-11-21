namespace TurretGame.Application.Systems;

public class ResourceManager
{
    public int ResourceCount { get; private set; }

    public ResourceManager()
    {
        ResourceCount = 0;
    }

    public void CollectResource(int amount)
    {
        ResourceCount += amount;
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

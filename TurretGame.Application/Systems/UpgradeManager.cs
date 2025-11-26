using System;
using System.Collections.Generic;
using TurretGame.Core.Upgrades;

namespace TurretGame.Application.Systems;

public class UpgradeManager
{
    private readonly UpgradeState _upgradeState;
    private readonly Random _random;
    private List<IUpgrade> _currentOptions;

    // Pool of all available upgrades
    private readonly List<IUpgrade> _upgradePool = new()
    {
        new ExtraTurretUpgrade(),
        new ProjectileVelocityUpgrade(),
        new ResourceValueUpgrade()
    };

    public UpgradeState UpgradeState => _upgradeState;
    public IReadOnlyList<IUpgrade> CurrentOptions => _currentOptions;

    public UpgradeManager()
    {
        _upgradeState = new UpgradeState();
        _random = new Random();
        _currentOptions = new List<IUpgrade>();
    }

    /// <summary>
    /// Generates 3 random upgrade options for the player to choose from
    /// </summary>
    public void GenerateUpgradeOptions()
    {
        _currentOptions = new List<IUpgrade>();

        // Create a copy of the pool to sample from
        var availableUpgrades = new List<IUpgrade>(_upgradePool);

        // Select 3 random upgrades (or all if pool is smaller)
        int optionsToGenerate = Math.Min(3, availableUpgrades.Count);
        for (int i = 0; i < optionsToGenerate; i++)
        {
            int randomIndex = _random.Next(availableUpgrades.Count);
            _currentOptions.Add(availableUpgrades[randomIndex]);
            availableUpgrades.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// Applies the selected upgrade to the upgrade state
    /// </summary>
    public void SelectUpgrade(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= _currentOptions.Count)
        {
            return;
        }

        var selectedUpgrade = _currentOptions[optionIndex];
        selectedUpgrade.Apply(_upgradeState);
        _currentOptions.Clear();
    }

    public void Reset()
    {
        _upgradeState.Reset();
        _currentOptions.Clear();
    }
}

using System;
using System.Collections.Generic;
using TurretGame.Core.Upgrades;

namespace TurretGame.Application.Systems;

public class UpgradeManager
{
    private readonly UpgradeState _upgradeState;
    private readonly Random _random;
    private List<IUpgrade> _currentOptions;
    private Dictionary<Type, int> _upgradeSelectionCounts;

    // Pool of all available upgrades
    private readonly List<IUpgrade> _upgradePool = new()
    {
        new ExtraTurretUpgrade(),
        new ProjectileVelocityUpgrade(),
        new ResourceValueUpgrade(),
        new SlowHunterUpgrade(),
        new SlowPreyUpgrade()
    };

    // Maximum times each upgrade can be selected (null = unlimited)
    private readonly Dictionary<Type, int> _upgradeMaxSelections = new()
    {
        { typeof(ExtraTurretUpgrade), 2 }  // Limit to 2 selections (max 3 turrets per round)
    };

    public UpgradeState UpgradeState => _upgradeState;
    public IReadOnlyList<IUpgrade> CurrentOptions => _currentOptions;

    public UpgradeManager()
    {
        _upgradeState = new UpgradeState();
        _random = new Random();
        _currentOptions = new List<IUpgrade>();
        _upgradeSelectionCounts = new Dictionary<Type, int>();
    }

    /// <summary>
    /// Generates 2 random upgrade options for the player to choose from
    /// Extra Turret upgrade is always included until exhausted
    /// </summary>
    /// <param name="currentTurretCount">Current number of turrets on the board (unused, kept for compatibility)</param>
    /// <param name="maxTurrets">Maximum allowed turrets (unused, kept for compatibility)</param>
    public void GenerateUpgradeOptions(int currentTurretCount = 0, int maxTurrets = int.MaxValue)
    {
        _currentOptions = new List<IUpgrade>();

        // Check if Extra Turret upgrade is still available
        IUpgrade? extraTurretUpgrade = null;
        var otherUpgrades = new List<IUpgrade>();

        foreach (var upgrade in _upgradePool)
        {
            var upgradeType = upgrade.GetType();
            bool isAvailable = true;

            // Check if this upgrade has a limit
            if (_upgradeMaxSelections.TryGetValue(upgradeType, out int maxSelections))
            {
                // Get current selection count
                int currentSelections = _upgradeSelectionCounts.GetValueOrDefault(upgradeType, 0);

                // Only available if below limit
                isAvailable = currentSelections < maxSelections;
            }

            if (isAvailable)
            {
                // Separate Extra Turret from other upgrades
                if (upgradeType == typeof(ExtraTurretUpgrade))
                {
                    extraTurretUpgrade = upgrade;
                }
                else
                {
                    otherUpgrades.Add(upgrade);
                }
            }
        }

        // Always include Extra Turret if available
        if (extraTurretUpgrade != null)
        {
            _currentOptions.Add(extraTurretUpgrade);
        }

        // Add 1 random other upgrade (or more if Extra Turret is exhausted)
        int remainingSlots = 2 - _currentOptions.Count;
        int optionsToGenerate = Math.Min(remainingSlots, otherUpgrades.Count);

        for (int i = 0; i < optionsToGenerate; i++)
        {
            int randomIndex = _random.Next(otherUpgrades.Count);
            _currentOptions.Add(otherUpgrades[randomIndex]);
            otherUpgrades.RemoveAt(randomIndex);
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

        // Track upgrade selection count
        var upgradeType = selectedUpgrade.GetType();
        if (_upgradeSelectionCounts.ContainsKey(upgradeType))
        {
            _upgradeSelectionCounts[upgradeType]++;
        }
        else
        {
            _upgradeSelectionCounts[upgradeType] = 1;
        }

        _currentOptions.Clear();
    }

    public void Reset()
    {
        _upgradeState.Reset();
        _currentOptions.Clear();
        _upgradeSelectionCounts.Clear();
    }
}

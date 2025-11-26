namespace TurretGame.Core.Upgrades;

/// <summary>
/// Tracks all active upgrades and their cumulative effects
/// </summary>
public class UpgradeState
{
    /// <summary>
    /// Number of extra turret placements per round (cumulative)
    /// </summary>
    public int ExtraTurretsPerRound { get; set; } = 0;

    /// <summary>
    /// Projectile velocity multiplier (1.0 = no bonus, 1.1 = 10% faster)
    /// </summary>
    public float ProjectileVelocityMultiplier { get; set; } = 1.0f;

    /// <summary>
    /// Resource value bonus (added to base value of 1)
    /// </summary>
    public int ResourceValueBonus { get; set; } = 0;

    /// <summary>
    /// Total resource value per pickup (1 + bonuses)
    /// </summary>
    public int ResourceValue => 1 + ResourceValueBonus;

    /// <summary>
    /// Hunter (chase enemy) speed multiplier (1.0 = no change, 0.95 = 5% slower)
    /// </summary>
    public float HunterSpeedMultiplier { get; set; } = 1.0f;

    /// <summary>
    /// Prey (fleeing enemy) speed multiplier (1.0 = no change, 0.95 = 5% slower)
    /// </summary>
    public float PreySpeedMultiplier { get; set; } = 1.0f;

    public void Reset()
    {
        ExtraTurretsPerRound = 0;
        ProjectileVelocityMultiplier = 1.0f;
        ResourceValueBonus = 0;
        HunterSpeedMultiplier = 1.0f;
        PreySpeedMultiplier = 1.0f;
    }
}

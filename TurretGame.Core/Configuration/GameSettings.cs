namespace TurretGame.Core.Configuration;

public class GameSettings
{
    /// <summary>
    /// Player movement speed in pixels per second
    /// </summary>
    public float PlayerSpeed { get; set; } = 400f;

    /// <summary>
    /// Hunter enemy speed in pixels per second
    /// </summary>
    public float HunterSpeed { get; set; } = 180f;

    /// <summary>
    /// Prey enemy speed in pixels per second
    /// </summary>
    public float PreySpeed { get; set; } = 320f;

    /// <summary>
    /// Number of hunters spawned per wave (multiplied by wave number)
    /// </summary>
    public int HuntersPerWave { get; set; } = 1;
}

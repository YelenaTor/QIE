namespace Quartermaster.Configuration;

/// <summary>
/// Submarine module preferences.
/// </summary>
public class SubmarineConfig
{
    public float AlertThresholdGilPerHour { get; set; } = 50_000;
    public bool AutoSuggest { get; set; } = true;
}

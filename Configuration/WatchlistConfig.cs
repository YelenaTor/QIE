namespace Quartermaster.Configuration;

/// <summary>
/// Per-item watchlist threshold configuration.
/// </summary>
public class WatchlistConfig
{
    public decimal DefaultMinProfitGil { get; set; } = 10_000;
    public float DefaultMinProfitPercent { get; set; } = 10f;
    public float DefaultMinSaleVelocityPerDay { get; set; } = 1f;
    public float DefaultMaxCompetitionScore { get; set; } = 9999f;
    public bool DefaultAlertEnabled { get; set; } = true;
    public bool DefaultAutoCraftEnabled { get; set; } = false;
}

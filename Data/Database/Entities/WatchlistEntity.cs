namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Persisted watchlist entry.
/// </summary>
public class WatchlistEntity
{
    public uint ItemId { get; set; }
    public uint WorldId { get; set; }
    public decimal MinProfitGil { get; set; }
    public float MinProfitPercent { get; set; }
    public float MinVelocityPerDay { get; set; }
    public float MaxCompetitionScore { get; set; } = 9999;
    public bool AlertEnabled { get; set; } = true;
    public bool AutoCraftEnabled { get; set; }
    public DateTimeOffset AddedAt { get; set; }
}

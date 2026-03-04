namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Price snapshots over time (rolling window, pruned at 30 days).
/// </summary>
public class PriceHistoryEntity
{
    public long Id { get; set; }
    public uint ItemId { get; set; }
    public uint WorldId { get; set; }
    public int MinPrice { get; set; }
    public int AvgPrice { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset SnappedAt { get; set; }
}

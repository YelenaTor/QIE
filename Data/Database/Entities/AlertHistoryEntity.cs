namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Fired alerts log (audit, kept indefinitely unless user clears).
/// </summary>
public class AlertHistoryEntity
{
    public string Id { get; set; } = string.Empty; // GUID
    public uint ItemId { get; set; }
    public string Severity { get; set; } = string.Empty;
    public decimal ProjectedProfit { get; set; }
    public float SaleVelocity { get; set; }
    public float CompetitionScore { get; set; }
    public int SuggestedQty { get; set; }
    public DateTimeOffset FiredAt { get; set; }
    public bool Acknowledged { get; set; }
    public bool ActedOn { get; set; }
}

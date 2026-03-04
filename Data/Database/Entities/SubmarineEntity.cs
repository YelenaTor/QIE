namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Submarine sector data (seeded from community tables, user can override).
/// </summary>
public class SubmarineEntity
{
    public int SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public float AverageGilPerHour { get; set; }
    public int VoyageDurationMin { get; set; }
    public string DropTableJson { get; set; } = string.Empty; // JSON: [{itemId, dropRate}]
    public DateTimeOffset UpdatedAt { get; set; }
}

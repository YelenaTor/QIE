namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Venture history populated via AutoRetainer IPC or manual entry.
/// </summary>
public class VentureHistoryEntity
{
    public long Id { get; set; }
    public string RetainerName { get; set; } = string.Empty;
    public int VentureTypeId { get; set; }
    public uint? ItemId { get; set; }
    public int? Quantity { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
    public uint WorldId { get; set; }
}

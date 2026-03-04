namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Cached item metadata from XIVAPI.
/// </summary>
public class ItemEntity
{
    public uint ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int IconId { get; set; }
    public bool CanBeCrafted { get; set; }
    public int? VendorPrice { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

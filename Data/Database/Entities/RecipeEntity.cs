namespace Quartermaster.Data.Database.Entities;

/// <summary>
/// Cached recipe graphs from XIVAPI.
/// </summary>
public class RecipeEntity
{
    public uint RecipeId { get; set; }
    public uint ItemId { get; set; }
    public int ResultQty { get; set; }
    public string Ingredients { get; set; } = string.Empty; // JSON: [{itemId, qty, isNpc}]
    public DateTimeOffset CachedAt { get; set; }
}

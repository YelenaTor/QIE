using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Cache;

/// <summary>
/// In-memory recipe graph cache.
/// </summary>
public class RecipeCache
{
    private readonly Dictionary<uint, CacheEntry<RecipeGraph>> _store = new();
    private readonly TimeSpan _ttl = TimeSpan.FromHours(24);

    public bool TryGet(uint itemId, out RecipeGraph? recipe)
    {
        if (_store.TryGetValue(itemId, out var entry) && !entry.IsExpired(_ttl))
        {
            recipe = entry.Value;
            return true;
        }

        recipe = null;
        return false;
    }

    public void Set(uint itemId, RecipeGraph recipe)
    {
        _store[itemId] = new CacheEntry<RecipeGraph>(recipe, DateTimeOffset.UtcNow);
    }

    public void Invalidate(uint itemId)
    {
        _store.Remove(itemId);
    }

    public void Clear()
    {
        _store.Clear();
    }
}

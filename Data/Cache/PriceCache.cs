using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Cache;

/// <summary>
/// TTL-based Dictionary&lt;(uint, uint), PriceSnapshot&gt; cache.
/// </summary>
public class PriceCache
{
    private readonly Dictionary<(uint itemId, uint worldId), CacheEntry<PriceSnapshot>> _store = new();
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public bool TryGet(uint itemId, uint worldId, out PriceSnapshot? snapshot)
    {
        if (_store.TryGetValue((itemId, worldId), out var entry) && !entry.IsExpired(_ttl))
        {
            snapshot = entry.Value;
            return true;
        }

        snapshot = null;
        return false;
    }

    public void Set(uint itemId, uint worldId, PriceSnapshot snapshot)
    {
        _store[(itemId, worldId)] = new CacheEntry<PriceSnapshot>(snapshot, DateTimeOffset.UtcNow);
    }

    public void Invalidate(uint itemId, uint worldId)
    {
        _store.Remove((itemId, worldId));
    }
}

public record CacheEntry<T>(T Value, DateTimeOffset CachedAt)
{
    public bool IsExpired(TimeSpan ttl) => DateTimeOffset.UtcNow - CachedAt > ttl;
}

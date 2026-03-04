using Quartermaster.Data.Database;
using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly QuartermasterDb _db;

    public WatchlistRepository(QuartermasterDb db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<WatchlistEntry>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<WatchlistEntry?> GetByItemIdAsync(uint itemId, uint worldId)
    {
        throw new NotImplementedException();
    }

    public Task UpsertAsync(WatchlistEntry entry)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(uint itemId, uint worldId)
    {
        throw new NotImplementedException();
    }
}

using Quartermaster.Data.Database;
using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly QuartermasterDb _db;

    public PriceRepository(QuartermasterDb db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<PriceSnapshot>> GetHistoryAsync(uint itemId, uint worldId, int limit = 100)
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync(PriceSnapshot snapshot)
    {
        throw new NotImplementedException();
    }

    public Task PruneOlderThanAsync(DateTimeOffset cutoff)
    {
        throw new NotImplementedException();
    }
}

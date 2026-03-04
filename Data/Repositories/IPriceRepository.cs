using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public interface IPriceRepository
{
    Task<IReadOnlyList<PriceSnapshot>> GetHistoryAsync(uint itemId, uint worldId, int limit = 100);
    Task SaveSnapshotAsync(PriceSnapshot snapshot);
    Task PruneOlderThanAsync(DateTimeOffset cutoff);
}

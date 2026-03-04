using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public interface IWatchlistRepository
{
    Task<IReadOnlyList<WatchlistEntry>> GetAllAsync();
    Task<WatchlistEntry?> GetByItemIdAsync(uint itemId, uint worldId);
    Task UpsertAsync(WatchlistEntry entry);
    Task RemoveAsync(uint itemId, uint worldId);
}

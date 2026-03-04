using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public interface IAlertRepository
{
    Task<IReadOnlyList<AlertEvent>> GetRecentAsync(int limit = 50);
    Task SaveAsync(AlertEvent alert);
    Task AcknowledgeAsync(Guid alertId);
    Task ClearAllAsync();
}

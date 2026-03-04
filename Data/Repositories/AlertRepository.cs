using Quartermaster.Data.Database;
using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly QuartermasterDb _db;

    public AlertRepository(QuartermasterDb db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<AlertEvent>> GetRecentAsync(int limit = 50)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(AlertEvent alert)
    {
        throw new NotImplementedException();
    }

    public Task AcknowledgeAsync(Guid alertId)
    {
        throw new NotImplementedException();
    }

    public Task ClearAllAsync()
    {
        throw new NotImplementedException();
    }
}

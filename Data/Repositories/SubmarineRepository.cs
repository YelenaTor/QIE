using Quartermaster.Data.Database;
using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public class SubmarineRepository : ISubmarineRepository
{
    private readonly QuartermasterDb _db;

    public SubmarineRepository(QuartermasterDb db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<SubmarineRoute>> GetAllSectorsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SubmarineRoute?> GetSectorAsync(int sectorId)
    {
        throw new NotImplementedException();
    }

    public Task UpsertSectorAsync(SubmarineRoute route)
    {
        throw new NotImplementedException();
    }
}

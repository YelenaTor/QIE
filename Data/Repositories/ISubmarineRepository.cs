using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public interface ISubmarineRepository
{
    Task<IReadOnlyList<SubmarineRoute>> GetAllSectorsAsync();
    Task<SubmarineRoute?> GetSectorAsync(int sectorId);
    Task UpsertSectorAsync(SubmarineRoute route);
}

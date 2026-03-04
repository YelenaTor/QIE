using Quartermaster.Domain.Models;

namespace Quartermaster.Engine;

/// <summary>
/// Local listing-based competition score (derived from listing count heuristic).
/// </summary>
public class CompetitionEstimator
{
    public Task<CompetitionScore> EstimateAsync(uint itemId, uint worldId)
    {
        throw new NotImplementedException();
    }
}

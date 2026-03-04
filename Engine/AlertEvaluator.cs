using Quartermaster.Domain.Models;

namespace Quartermaster.Engine;

/// <summary>
/// Stateless — takes a watchlist entry and a profit result, returns an alert or null.
/// Returns null if thresholds not met; returns AlertEvent if any threshold is crossed.
/// </summary>
public class AlertEvaluator
{
    public AlertEvent? Evaluate(WatchlistEntry entry, CraftProfitResult profit)
    {
        throw new NotImplementedException();
    }
}

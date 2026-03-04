using Quartermaster.Domain.Models;

namespace Quartermaster.Engine;

/// <summary>
/// The main background loop. Coordinates polling → cache → calculation → evaluation → alert dispatch.
/// Phase 4: logging stub — processes nothing yet, just ticks and logs.
/// </summary>
public class WatchlistProcessor
{
    public event EventHandler<AlertEvent>? AlertFired;

    private int _tickCount;

    /// <summary>
    /// Called by BackgroundPoller on tick.
    /// Phase 4: just logs and increments the tick counter.
    /// Real implementation in Phase 5 (Intelligence Core).
    /// </summary>
    public Task ProcessAsync(CancellationToken ct = default)
    {
        _tickCount++;
        Plugin.Log.Debug($"[QM] WatchlistProcessor tick #{_tickCount}");

        // Phase 5 will:
        // 1. Load all watchlist entries from DB
        // 2. For each entry, check cache (PriceCache)
        // 3. If expired, poll Universalis
        // 4. Run ProfitCalculator
        // 5. Run AlertEvaluator
        // 6. DispatchAlert for any crossed thresholds

        return Task.CompletedTask;
    }

    private Task ProcessEntryAsync(WatchlistEntry entry)
    {
        // Phase 5 implementation
        return Task.CompletedTask;
    }

    private void DispatchAlert(AlertEvent alert)
    {
        Plugin.Log.Information($"[QM] Alert fired: {alert.ItemName} — {alert.Severity}");
        AlertFired?.Invoke(this, alert);
    }
}

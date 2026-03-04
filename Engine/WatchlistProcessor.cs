using Quartermaster.Domain.Models;

namespace Quartermaster.Engine;

/// <summary>
/// The main background loop. Coordinates polling → cache → calculation → evaluation → alert dispatch.
/// </summary>
public class WatchlistProcessor
{
    public event EventHandler<AlertEvent>? AlertFired;

    /// <summary>
    /// Called by BackgroundPoller on tick.
    /// </summary>
    public Task ProcessAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    private Task ProcessEntryAsync(WatchlistEntry entry)
    {
        throw new NotImplementedException();
    }

    private void DispatchAlert(AlertEvent alert)
    {
        AlertFired?.Invoke(this, alert);
    }
}

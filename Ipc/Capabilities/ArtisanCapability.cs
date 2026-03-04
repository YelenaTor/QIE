namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// Send craft list, check readiness via Artisan IPC.
/// Failures are caught and logged — never propagated up.
/// </summary>
public class ArtisanCapability
{
    /// <summary>
    /// Returns false if Artisan is busy or unavailable.
    /// </summary>
    public bool IsAvailable()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Enqueues a list of itemId/quantity pairs for crafting.
    /// Returns false if Artisan rejected the request.
    /// </summary>
    public bool TryEnqueueCraftList(IEnumerable<(uint itemId, int quantity)> items)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// True if Artisan is currently mid-craft.
    /// </summary>
    public bool IsCrafting()
    {
        throw new NotImplementedException();
    }
}

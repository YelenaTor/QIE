namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// NPC price lookups via ItemVendorLocation IPC.
/// </summary>
public class ItemVendorCapability
{
    /// <summary>
    /// Get NPC vendor price for an item, or null if not vendor-purchasable.
    /// </summary>
    public int? GetVendorPrice(uint itemId)
    {
        throw new NotImplementedException();
    }
}

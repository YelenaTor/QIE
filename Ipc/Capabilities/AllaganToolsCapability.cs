namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// Inventory queries via AllaganTools IPC.
/// </summary>
public class AllaganToolsCapability
{
    /// <summary>
    /// Get the count of a specific item across all inventories.
    /// </summary>
    public int GetItemCount(uint itemId)
    {
        throw new NotImplementedException();
    }
}

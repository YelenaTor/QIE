namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// Queue gather request via GatherBuddyReborn IPC.
/// vnavmesh required for full auto; falls back to map waypoint if absent.
/// </summary>
public class GatherBuddyCapability
{
    /// <summary>
    /// Queue a gather request for an item.
    /// </summary>
    public bool TryQueueGather(uint itemId, int quantity)
    {
        throw new NotImplementedException();
    }
}

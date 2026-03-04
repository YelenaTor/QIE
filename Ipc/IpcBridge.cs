using Quartermaster.Domain.Enums;
using Quartermaster.Ipc.Capabilities;

namespace Quartermaster.Ipc;

/// <summary>
/// Capability detection + facade. The only layer that touches Dalamud IPC APIs.
/// Everything above operates on domain types.
/// </summary>
public class IpcBridge
{
    public IpcCapability DetectedCapabilities { get; private set; } = IpcCapability.None;

    public ArtisanCapability? Artisan { get; private set; }
    public AutoRetainerCapability? AutoRetainer { get; private set; }
    public GatherBuddyCapability? GatherBuddy { get; private set; }
    public AllaganToolsCapability? AllaganTools { get; private set; }
    public ItemVendorCapability? ItemVendor { get; private set; }
    public NotificationMasterCapability? NotificationMaster { get; private set; }

    /// <summary>
    /// Called on plugin load and re-evaluated on Framework.Update periodically.
    /// Cheap check — no alloc, flag compare only.
    /// </summary>
    public void RefreshCapabilities()
    {
        // TODO: Detect installed plugins and initialize capability wrappers
    }

    public bool Has(IpcCapability capability) =>
        DetectedCapabilities.HasFlag(capability);
}

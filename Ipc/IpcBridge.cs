using Dalamud.Plugin.Ipc;
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

    private DateTimeOffset _lastCheck = DateTimeOffset.MinValue;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Called on plugin load and re-evaluated on Framework.Update periodically.
    /// Cheap check — skips if last check was less than 30 seconds ago.
    /// </summary>
    public void RefreshCapabilities()
    {
        if (DateTimeOffset.UtcNow - _lastCheck < CheckInterval)
            return;

        _lastCheck = DateTimeOffset.UtcNow;
        var caps = IpcCapability.None;

        // Artisan
        if (TryDetect("Artisan.IsAvailable"))
        {
            caps |= IpcCapability.Artisan;
            Artisan ??= new ArtisanCapability();
        }
        else { Artisan = null; }

        // AutoRetainer
        if (TryDetect("AutoRetainer.GetOfflineCharacterData"))
        {
            caps |= IpcCapability.AutoRetainer;
            AutoRetainer ??= new AutoRetainerCapability();
        }
        else { AutoRetainer = null; }

        // GatherBuddy
        if (TryDetect("GatherBuddyReborn.IsAvailable"))
        {
            caps |= IpcCapability.GatherBuddy;
            GatherBuddy ??= new GatherBuddyCapability();
        }
        else { GatherBuddy = null; }

        // AllaganTools
        if (TryDetect("AllaganTools.GetCharacterItems"))
        {
            caps |= IpcCapability.AllaganTools;
            AllaganTools ??= new AllaganToolsCapability();
        }
        else { AllaganTools = null; }

        // ItemVendorLocation
        if (TryDetect("ItemVendorLocation.GetNearestVendor"))
        {
            caps |= IpcCapability.ItemVendorLocation;
            ItemVendor ??= new ItemVendorCapability();
        }
        else { ItemVendor = null; }

        // NotificationMaster
        if (TryDetect("NotificationMaster.ShowNotification"))
        {
            caps |= IpcCapability.NotificationMaster;
            NotificationMaster ??= new NotificationMasterCapability();
        }
        else { NotificationMaster = null; }

        if (caps != DetectedCapabilities)
        {
            Plugin.Log.Information($"[QM] IPC capabilities changed: {DetectedCapabilities} → {caps}");
            DetectedCapabilities = caps;
        }
    }

    public bool Has(IpcCapability capability) =>
        DetectedCapabilities.HasFlag(capability);

    /// <summary>
    /// Attempt to subscribe to an IPC channel. Returns true if the provider exists.
    /// </summary>
    private static bool TryDetect(string channelName)
    {
        try
        {
            // Attempt to get a subscriber — if the provider doesn't exist, this throws
            Plugin.PluginInterface.GetIpcSubscriber<bool>(channelName);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

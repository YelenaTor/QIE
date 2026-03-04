namespace Quartermaster.Ipc;

/// <summary>
/// Plugin name strings and IPC method names.
/// </summary>
public static class IpcConstants
{
    // Plugin internal names
    public const string ArtisanPlugin = "Artisan";
    public const string AutoRetainerPlugin = "AutoRetainer";
    public const string GatherBuddyPlugin = "GatherBuddyReborn";
    public const string AllaganToolsPlugin = "AllaganTools";
    public const string ItemVendorPlugin = "ItemVendorLocation";
    public const string NotificationMasterPlugin = "NotificationMaster";
    public const string VNavMeshPlugin = "vnavmesh";

    // IPC method name patterns
    public static class Artisan
    {
        public const string AddItemsToQueue = "Artisan.AddItemsToQueue";
        public const string IsCrafting = "Artisan.IsCrafting";
        public const string IsAvailable = "Artisan.IsAvailable";
    }

    public static class AutoRetainer
    {
        public const string SetSuppressed = "AutoRetainer.SetSuppressed";
        public const string GetRetainerData = "AutoRetainer.GetRetainerData";
        public const string GetSubmarineData = "AutoRetainer.GetSubmarineData";
    }

    public static class GatherBuddy
    {
        public const string InvokeGather = "GatherBuddyReborn.InvokeGather";
    }

    public static class AllaganTools
    {
        public const string GetItemCount = "AllaganTools.GetItemCount";
    }

    public static class ItemVendor
    {
        public const string GetVendorPrice = "ItemVendorLocation.GetVendorPrice";
    }

    public static class NotifMaster
    {
        public const string SendToast = "NotificationMaster.SendToast";
    }
}

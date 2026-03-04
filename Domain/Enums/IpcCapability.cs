namespace Quartermaster.Domain.Enums;

[Flags]
public enum IpcCapability : uint
{
    None               = 0,
    Artisan            = 1 << 0,
    AutoRetainer       = 1 << 1,
    GatherBuddy        = 1 << 2,
    AllaganTools       = 1 << 3,
    ItemVendorLocation = 1 << 4,
    NotificationMaster = 1 << 5,
    VNavMesh           = 1 << 6,
}

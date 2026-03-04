namespace Quartermaster.Util;

/// <summary>
/// World/datacenter resolution helpers.
/// </summary>
public static class WorldHelper
{
    /// <summary>
    /// Get world name from world ID using Dalamud data.
    /// </summary>
    public static string GetWorldName(uint worldId)
    {
        // TODO: Resolve via Services.DataManager.GetExcelSheet<World>()
        return $"World_{worldId}";
    }

    /// <summary>
    /// Get datacenter name for a world.
    /// </summary>
    public static string GetDatacenterName(uint worldId)
    {
        // TODO: Resolve via Lumina Excel sheets
        return $"DC_{worldId}";
    }

    /// <summary>
    /// Get the current player's home world ID.
    /// </summary>
    public static uint GetHomeWorldId()
    {
        // TODO: Services.ClientState.LocalPlayer?.HomeWorld.Id ?? 0
        return 0;
    }
}

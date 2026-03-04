namespace Quartermaster.Util;

/// <summary>
/// DalamudReflector plugin presence checks.
/// </summary>
public static class PluginDetector
{
    /// <summary>
    /// Check if a plugin is installed and loaded by its internal name.
    /// </summary>
    public static bool IsPluginLoaded(string pluginInternalName)
    {
        // TODO: Use DalamudReflector or plugin list API to check presence
        return false;
    }
}

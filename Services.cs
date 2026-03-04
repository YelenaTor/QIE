namespace Quartermaster;

/// <summary>
/// Static helpers that reference the plugin's static service properties.
/// With the Candy Coat / ECommons pattern, Dalamud services are accessed via
/// Plugin.[ServiceName] or ECommons.DalamudServices.Svc.[ServiceName].
/// This file provides additional convenience accessors if needed.
/// </summary>
public static class Services
{
    /// <summary>
    /// Convenience accessor for the plugin interface.
    /// Prefer Plugin.PluginInterface or Svc.PluginInterface directly.
    /// </summary>
    public static Dalamud.Plugin.IDalamudPluginInterface PluginInterface => Plugin.PluginInterface;
}

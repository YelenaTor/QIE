namespace Quartermaster.UI.Panels;

/// <summary>
/// Anchor side toggle, thresholds, IPC status indicators, data cache controls.
/// </summary>
public class SettingsPanel : IDrawerPanel
{
    public string Id => "settings";
    public string Title => "Settings";
    public string Icon => "\uF013"; // FontAwesome cog
    public string Tooltip => "Settings — configure Quartermaster";
    public bool HasUnreadBadge => false;

    public void Draw()
    {
        // TODO: UI: anchor side toggle (Left / Right), bar opacity
        // TODO: Engine: poll interval, world selection, currency display
        // TODO: IPC Status panel: detected plugins with green/grey/red indicators
        // TODO: Data: cache age display, [Clear cache] [Re-seed submarine data]
        // TODO: About: version, links, credits
    }
}

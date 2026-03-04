namespace Quartermaster.UI.Panels;

/// <summary>
/// Venture status, reassignment suggestions.
/// </summary>
public class RetainerPanel : IDrawerPanel
{
    public string Id => "retainers";
    public string Title => "Retainers";
    public string Icon => "\uF007"; // FontAwesome user
    public string Tooltip => "Retainers — venture management";
    public bool HasUnreadBadge => false;

    public void Draw()
    {
        // TODO: Per-retainer cards (name, current venture, time remaining)
        // TODO: Reassignment suggestions (current vs suggested, profit delta)
        // TODO: [Reassign] button (requires AutoRetainer IPC + user confirm)
    }
}

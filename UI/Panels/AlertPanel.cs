namespace Quartermaster.UI.Panels;

/// <summary>
/// Alert feed with action buttons. Newest-first ordering.
/// </summary>
public class AlertPanel : IDrawerPanel
{
    public string Id => "alerts";
    public string Title => "Alerts";
    public string Icon => "\uF0F3"; // FontAwesome bell
    public string Tooltip => "Alerts — profit opportunity notifications";
    public bool HasUnreadBadge => _unreadCount > 0;

    private int _unreadCount;

    public void Draw()
    {
        // TODO: Filter bar: All / Opportunity / Warning / Urgent
        // TODO: Alert cards (newest first)
        // TODO: Action buttons: [Craft] [Gather] [List] [Dismiss]
        // TODO: "Clear all" button
    }
}

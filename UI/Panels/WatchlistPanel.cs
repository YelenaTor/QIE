namespace Quartermaster.UI.Panels;

/// <summary>
/// Add/remove items, edit thresholds inline, show current profit state per item.
/// </summary>
public class WatchlistPanel : IDrawerPanel
{
    public string Id => "watchlist";
    public string Title => "Watchlist";
    public string Icon => "\uF03A"; // FontAwesome list
    public string Tooltip => "Watchlist — track item profitability";
    public bool HasUnreadBadge => false;

    public void Draw()
    {
        // TODO: Search/add item bar
        // TODO: Item list rows (icon, name, current profit, trend arrow, active badge)
        // TODO: Expand row → inline threshold editor
    }
}

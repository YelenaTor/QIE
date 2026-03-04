using Una.Drawing;

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

    private Node? _rootNode;

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("watchlist-panel.xml");
        _rootNode = doc.RootNode!;

        var searchPlaceholder = _rootNode.FindById("SearchPlaceholder");
        if (searchPlaceholder is not null)
            searchPlaceholder.NodeValue = "Search items to track...";

        var emptyIcon = _rootNode.QuerySelector(".empty-icon");
        if (emptyIcon is not null)
            emptyIcon.NodeValue = "\uF002"; // search icon

        var emptyText = _rootNode.QuerySelector(".empty-text");
        if (emptyText is not null)
            emptyText.NodeValue = "No items on your watchlist yet.\nSearch above to add items.";

        // Items will be populated dynamically when data is available
        return _rootNode;
    }
}

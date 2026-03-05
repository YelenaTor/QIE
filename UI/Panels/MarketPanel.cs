using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Deep-dive into a single item's market data: price summary, listings, recipe tree.
/// </summary>
public class MarketPanel : IDrawerPanel
{
    public string Id => "market";
    public string Title => "Market";
    public string Icon => "\uF201"; // FontAwesome chart-line
    public string Tooltip => "Market — item price analysis";
    public bool HasUnreadBadge => false;

    private Node? _rootNode;

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("market-panel.xml");
        _rootNode = doc.RootNode!;

        // Set section headers
        var sections = _rootNode.QuerySelectorAll(".qm-section-header").ToList();
        if (sections.Count >= 2)
        {
            sections[0].NodeValue = "CURRENT LISTINGS";
            sections[1].NodeValue = "RECIPE INGREDIENTS";
        }

        // Set price summary labels
        var priceCells = _rootNode.QuerySelectorAll(".price-label").ToList();
        if (priceCells.Count >= 4)
        {
            priceCells[0].NodeValue = "MIN PRICE";
            priceCells[1].NodeValue = "AVG PRICE";
            priceCells[2].NodeValue = "VELOCITY";
            priceCells[3].NodeValue = "LISTINGS";
        }

        // Default placeholders
        SetNodeText("PriceMin", "—");
        SetNodeText("PriceAvg", "—");
        SetNodeText("PriceVelocity", "—");
        SetNodeText("PriceListings", "—");

        // Empty state
        var emptyIcon = _rootNode.QuerySelector(".empty-icon");
        if (emptyIcon is not null) emptyIcon.NodeValue = "\uF201";

        var emptyText = _rootNode.QuerySelector(".empty-text");
        if (emptyText is not null) emptyText.NodeValue = "Select an item from the Watchlist\nto view market data.";

        return _rootNode;
    }

    private void SetNodeText(string id, string text)
    {
        var node = _rootNode?.FindById(id);
        if (node is not null) node.NodeValue = text;
    }
}

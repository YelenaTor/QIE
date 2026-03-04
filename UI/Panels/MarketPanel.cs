namespace Quartermaster.UI.Panels;

/// <summary>
/// Item detail: price chart, cost breakdown, ingredient tree.
/// </summary>
public class MarketPanel : IDrawerPanel
{
    public string Id => "market";
    public string Title => "Market";
    public string Icon => "\uF201"; // FontAwesome chart-line
    public string Tooltip => "Market — item price analysis";
    public bool HasUnreadBadge => false;

    public void Draw()
    {
        // TODO: Item selector (defaults to last-viewed)
        // TODO: Price chart (ImPlot — 7d history, zoom-able)
        // TODO: Current listings table (qty, price, retainer, world)
        // TODO: Ingredient cost tree (recursive, collapsible nodes)
        // TODO: Profit summary chip
    }
}

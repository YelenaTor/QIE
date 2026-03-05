using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Alert feed — actionable alerts sorted by severity with filter buttons.
/// </summary>
public class AlertPanel : IDrawerPanel
{
    public string Id => "alerts";
    public string Title => "Alerts";
    public string Icon => "\uF0F3"; // FontAwesome bell
    public string Tooltip => "Alerts — actionable market opportunities";
    public bool HasUnreadBadge => _unreadCount > 0;

    private int _unreadCount;
    private Node? _rootNode;
    private string _activeFilter = "all";

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("alert-panel.xml");
        _rootNode = doc.RootNode!;

        // Set filter button labels
        SetNodeText("FilterAll", "All");
        SetNodeText("FilterOpportunity", "Opportunity");
        SetNodeText("FilterWarning", "Warning");
        SetNodeText("FilterUrgent", "Urgent");

        // Wire filter click handlers
        WireFilter("FilterAll", "all");
        WireFilter("FilterOpportunity", "opportunity");
        WireFilter("FilterWarning", "warning");
        WireFilter("FilterUrgent", "urgent");

        // Empty state
        var emptyIcon = _rootNode.QuerySelector(".empty-icon");
        if (emptyIcon is not null) emptyIcon.NodeValue = "\uF0F3";

        var emptyText = _rootNode.QuerySelector(".empty-text");
        if (emptyText is not null) emptyText.NodeValue = "No alerts yet.\nAlerts appear when watchlist thresholds are crossed.";

        return _rootNode;
    }

    private void SetNodeText(string id, string text)
    {
        var node = _rootNode?.FindById(id);
        if (node is not null) node.NodeValue = text;
    }

    private void WireFilter(string id, string filter)
    {
        var node = _rootNode?.FindById(id);
        if (node is null) return;

        node.OnMouseUp += _ =>
        {
            _activeFilter = filter;
            UpdateFilterStates();
        };
    }

    private void UpdateFilterStates()
    {
        foreach (var f in new[] { ("FilterAll", "all"), ("FilterOpportunity", "opportunity"), ("FilterWarning", "warning"), ("FilterUrgent", "urgent") })
        {
            _rootNode?.FindById(f.Item1)?.ToggleClass("active", f.Item2 == _activeFilter);
        }
    }
}

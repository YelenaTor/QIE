using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Submarine sector rankings and route suggestions.
/// </summary>
public class SubmarinePanel : IDrawerPanel
{
    public string Id => "submarines";
    public string Title => "Submarines";
    public string Icon => "\uF21A"; // FontAwesome ship
    public string Tooltip => "Submarines — voyage profit advisor";
    public bool HasUnreadBadge => false;

    private Node? _rootNode;

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("submarine-panel.xml");
        _rootNode = doc.RootNode!;

        // Section header
        var header = _rootNode.QuerySelector(".qm-section-header");
        if (header is not null) header.NodeValue = "SECTOR RANKINGS";

        // Empty state
        var emptyIcon = _rootNode.QuerySelector(".empty-icon");
        if (emptyIcon is not null) emptyIcon.NodeValue = "\uF21A";

        var emptyText = _rootNode.QuerySelector(".empty-text");
        if (emptyText is not null) emptyText.NodeValue = "No submarine data available.\nRequires AutoRetainer integration.";

        return _rootNode;
    }
}

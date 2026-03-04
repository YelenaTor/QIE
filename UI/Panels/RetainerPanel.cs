using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Retainer venture status and reassignment suggestions.
/// </summary>
public class RetainerPanel : IDrawerPanel
{
    public string Id => "retainers";
    public string Title => "Retainers";
    public string Icon => "\uF007"; // FontAwesome user
    public string Tooltip => "Retainers — venture status & advice";
    public bool HasUnreadBadge => false;

    private Node? _rootNode;

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("retainer-panel.xml");
        _rootNode = doc.RootNode!;

        // Section header
        var header = _rootNode.QuerySelector(".qm-section-header");
        if (header is not null) header.NodeValue = "RETAINER VENTURES";

        // Empty state
        var emptyIcon = _rootNode.QuerySelector(".empty-icon");
        if (emptyIcon is not null) emptyIcon.NodeValue = "\uF007";

        var emptyText = _rootNode.QuerySelector(".empty-text");
        if (emptyText is not null) emptyText.NodeValue = "No retainer data available.\nRequires AutoRetainer integration.";

        return _rootNode;
    }
}

using Una.Drawing;

namespace Quartermaster.UI.Components;

/// <summary>
/// Colored pill showing profit margin. Returns a styled Node.
/// </summary>
public static class ProfitBadge
{
    public static Node Build(decimal profit)
    {
        var node = new Node { ClassList = { "qm-badge", "profit-badge" } };

        if (profit > 0)
        {
            node.ClassList.Add("positive");
            node.NodeValue = $"+{profit:N0}%";
        }
        else if (profit < 0)
        {
            node.ClassList.Add("negative");
            node.NodeValue = $"{profit:N0}%";
        }
        else
        {
            node.ClassList.Add("neutral");
            node.NodeValue = "0%";
        }

        return node;
    }
}

using Quartermaster.Domain.Enums;
using Una.Drawing;

namespace Quartermaster.UI.Components;

/// <summary>
/// Directional trend indicator. Returns a styled Node.
/// </summary>
public static class TrendArrow
{
    public static Node Build(MarketTrend trend)
    {
        var node = new Node { ClassList = { "trend-arrow" } };

        switch (trend)
        {
            case MarketTrend.Rising:
                node.ClassList.Add("rising");
                node.NodeValue = "\u2191"; // ↑
                break;
            case MarketTrend.Falling:
                node.ClassList.Add("falling");
                node.NodeValue = "\u2193"; // ↓
                break;
            case MarketTrend.Stable:
                node.ClassList.Add("stable");
                node.NodeValue = "\u2192"; // →
                break;
            case MarketTrend.Volatile:
                node.ClassList.Add("volatile");
                node.NodeValue = "\u2195"; // ↕
                break;
        }

        return node;
    }
}

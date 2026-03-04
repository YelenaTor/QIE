using Quartermaster.Domain.Enums;

namespace Quartermaster.UI.Components;

/// <summary>
/// Price trend indicator (↑ ↓ → ~).
/// </summary>
public class TrendArrow
{
    public void Draw(MarketTrend trend)
    {
        // TODO: Render colored arrow based on trend direction
        // Rising = green ↑, Falling = red ↓, Stable = grey →, Volatile = yellow ~
    }
}

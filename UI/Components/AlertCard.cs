using Quartermaster.Domain.Models;

namespace Quartermaster.UI.Components;

/// <summary>
/// Single alert row with action buttons (Craft, Gather, Dismiss, Pin).
/// </summary>
public class AlertCard
{
    public void Draw(AlertEvent alert, Action? onCraft = null, Action? onGather = null, Action? onDismiss = null)
    {
        // TODO: Item icon + name + severity color
        // TODO: Profit projection + velocity + competition
        // TODO: Action buttons: [Craft] [Gather] [List] [Dismiss]
    }
}

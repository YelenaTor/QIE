using Quartermaster.Domain.Enums;
using Quartermaster.Domain.Models;
using Una.Drawing;

namespace Quartermaster.UI.Components;

/// <summary>
/// Single alert row with action buttons. Returns a configured Node subtree
/// from the alert-card.xml template.
/// </summary>
public static class AlertCard
{
    public static Node Build(AlertEvent alert, Action? onCraft = null, Action? onGather = null, Action? onDismiss = null)
    {
        var doc = QuartermasterDrawing.LoadDocument("alert-card.xml");
        var root = doc.RootNode!;

        // Set item name
        var itemName = root.QuerySelector(".alert-item-name");
        if (itemName is not null) itemName.NodeValue = alert.ItemName;

        // Set severity icon class
        var severityIcon = root.QuerySelector(".alert-severity-icon");
        if (severityIcon is not null)
        {
            var severityClass = alert.Severity switch
            {
                AlertSeverity.Info => "info",
                AlertSeverity.Warning => "warning",
                AlertSeverity.Opportunity => "opportunity",
                AlertSeverity.Urgent => "urgent",
                _ => "info"
            };
            severityIcon.ClassList.Add(severityClass);
        }

        // Set profit text
        var profit = root.QuerySelector(".alert-profit");
        if (profit is not null) profit.NodeValue = alert.EstimatedProfit > 0
            ? $"+{alert.EstimatedProfit:N0}g"
            : "—";

        // Detail text
        var detail = root.QuerySelector(".alert-detail");
        if (detail is not null) detail.NodeValue = alert.Description;

        // Action buttons
        var actions = root.QuerySelector(".alert-card-actions");
        if (actions is not null)
        {
            if (onCraft is not null)
            {
                var craftBtn = new Node { ClassList = { "qm-button" } };
                craftBtn.NodeValue = "Craft";
                craftBtn.OnMouseUp += (_, _) => onCraft();
                actions.AppendChild(craftBtn);
            }

            if (onGather is not null)
            {
                var gatherBtn = new Node { ClassList = { "qm-button" } };
                gatherBtn.NodeValue = "Gather";
                gatherBtn.OnMouseUp += (_, _) => onGather();
                actions.AppendChild(gatherBtn);
            }

            if (onDismiss is not null)
            {
                var dismissBtn = new Node { ClassList = { "qm-button" } };
                dismissBtn.NodeValue = "Dismiss";
                dismissBtn.OnMouseUp += (_, _) => onDismiss();
                actions.AppendChild(dismissBtn);
            }
        }

        return root;
    }
}

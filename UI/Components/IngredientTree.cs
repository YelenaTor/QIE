using Quartermaster.Domain.Models;
using Una.Drawing;

namespace Quartermaster.UI.Components;

/// <summary>
/// Collapsible recipe cost breakdown. Builds Node rows from RecipeGraph data.
/// </summary>
public static class IngredientTree
{
    /// <summary>
    /// Build a single ingredient row Node.
    /// </summary>
    public static Node BuildRow(string name, int quantity, int unitCost, string source)
    {
        var row = new Node { ClassList = { "ingredient-row" } };

        var icon = new Node { ClassList = { "ingredient-icon" } };
        var nameNode = new Node { ClassList = { "ingredient-name", "qm-text" } };
        nameNode.NodeValue = name;

        var qtyNode = new Node { ClassList = { "ingredient-qty", "qm-text", "muted" } };
        qtyNode.NodeValue = $"x{quantity}";

        var costNode = new Node { ClassList = { "ingredient-cost", "qm-text" } };
        costNode.NodeValue = $"{unitCost * quantity:N0}g";

        var sourceNode = new Node { ClassList = { "ingredient-source", "qm-badge" } };
        sourceNode.NodeValue = source;

        // Add source-specific styling
        var sourceClass = source.ToLowerInvariant() switch
        {
            "npc" => "npc",
            "mb" or "market" => "mb",
            "gather" => "gather",
            _ => ""
        };
        if (!string.IsNullOrEmpty(sourceClass))
            sourceNode.ClassList.Add(sourceClass);

        row.AppendChild(icon);
        row.AppendChild(nameNode);
        row.AppendChild(qtyNode);
        row.AppendChild(costNode);
        row.AppendChild(sourceNode);

        return row;
    }
}

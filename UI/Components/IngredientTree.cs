using Quartermaster.Domain.Models;

namespace Quartermaster.UI.Components;

/// <summary>
/// Recursive recipe cost breakdown — collapsible tree nodes.
/// </summary>
public class IngredientTree
{
    public void Draw(IReadOnlyList<IngredientCost> ingredients)
    {
        // TODO: Render each ingredient as a tree node
        // TODO: Show item icon, name, quantity, unit cost, total cost
        // TODO: Highlight NPC-purchasable items differently
    }
}

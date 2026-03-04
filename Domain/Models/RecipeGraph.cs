namespace Quartermaster.Domain.Models;

public record IngredientNode(
    uint ItemId,
    string ItemName,
    int Quantity,
    bool IsNpcPurchasable,
    decimal UnitCost,
    IReadOnlyList<IngredientNode> Children
);

public record RecipeGraph(
    uint RecipeId,
    uint ResultItemId,
    int ResultQuantity,
    IReadOnlyList<IngredientNode> Ingredients
);

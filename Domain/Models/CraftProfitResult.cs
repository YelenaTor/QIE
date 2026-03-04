using Quartermaster.Domain.Enums;

namespace Quartermaster.Domain.Models;

public record IngredientCost(
    uint ItemId,
    string ItemName,
    int Quantity,
    decimal UnitCost,
    decimal TotalCost,
    bool IsNpcPrice
);

public record CraftProfitResult(
    uint ItemId,
    decimal MarketPrice,
    decimal CraftCost,
    decimal ProfitPerUnit,
    float ProfitMarginPercent,
    float SaleVelocityPerDay,
    float CompetitionScore,
    MarketTrend Trend,
    IReadOnlyList<IngredientCost> Ingredients
);

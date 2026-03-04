using Quartermaster.Domain.Models;

namespace Quartermaster.Engine;

/// <summary>
/// Core profit margin computation. Resolves a full ingredient tree recursively,
/// costing each node against either market price or NPC vendor price.
/// </summary>
public class ProfitCalculator
{
    /// <summary>
    /// Full recursive resolution — returns null if prices unavailable.
    /// </summary>
    public Task<CraftProfitResult?> CalculateAsync(uint itemId, uint worldId, int quantity = 1)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Shallow — only one level of ingredients (faster, less accurate).
    /// </summary>
    public Task<CraftProfitResult?> CalculateShallowAsync(uint itemId, uint worldId)
    {
        throw new NotImplementedException();
    }

    private Task<decimal> ResolveIngredientCostAsync(uint itemId, uint worldId, int quantity)
    {
        throw new NotImplementedException();
    }
}

using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public interface IRecipeRepository
{
    Task<RecipeGraph?> GetByItemIdAsync(uint itemId);
    Task<RecipeGraph?> GetByRecipeIdAsync(uint recipeId);
    Task SaveAsync(RecipeGraph recipe);
}

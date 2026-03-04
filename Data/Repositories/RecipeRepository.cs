using Quartermaster.Data.Database;
using Quartermaster.Domain.Models;

namespace Quartermaster.Data.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly QuartermasterDb _db;

    public RecipeRepository(QuartermasterDb db)
    {
        _db = db;
    }

    public Task<RecipeGraph?> GetByItemIdAsync(uint itemId)
    {
        throw new NotImplementedException();
    }

    public Task<RecipeGraph?> GetByRecipeIdAsync(uint recipeId)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(RecipeGraph recipe)
    {
        throw new NotImplementedException();
    }
}

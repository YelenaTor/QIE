namespace Quartermaster.Data.Http;

/// <summary>
/// XIVAPI client for recipe graphs and item metadata.
/// </summary>
public class XivApiClient
{
    private readonly HttpClient _httpClient;

    public XivApiClient(HttpClientFactory factory)
    {
        _httpClient = factory.GetClient();
    }

    public Task<RecipeResponse?> GetRecipeAsync(uint recipeId)
    {
        throw new NotImplementedException();
    }

    public Task<ItemResponse?> GetItemAsync(uint itemId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<RecipeResponse>> GetRecipesForItemAsync(uint itemId)
    {
        throw new NotImplementedException();
    }
}

// Response DTOs — stub types
public class RecipeResponse
{
    public uint RecipeId { get; set; }
    public uint ResultItemId { get; set; }
    public int ResultQuantity { get; set; }
    public IReadOnlyList<RecipeIngredient> Ingredients { get; set; } = Array.Empty<RecipeIngredient>();
}

public class RecipeIngredient
{
    public uint ItemId { get; set; }
    public int Quantity { get; set; }
    public bool IsNpc { get; set; }
}

public class ItemResponse
{
    public uint ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int IconId { get; set; }
    public bool CanBeCrafted { get; set; }
    public int? VendorPrice { get; set; }
}

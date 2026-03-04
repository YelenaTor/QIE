using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quartermaster.Data.Http;

/// <summary>
/// XIVAPI client for recipe graphs and item metadata.
/// Uses XIVAPI v2 (beta) — https://v2.xivapi.com/docs
/// </summary>
public class XivApiClient
{
    private const string BaseUrl = "https://v2.xivapi.com/api";
    private readonly HttpClientFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public XivApiClient(HttpClientFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Get a recipe by its ID.
    /// </summary>
    public async Task<RecipeResponse?> GetRecipeAsync(uint recipeId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/1/sheet/Recipe/{recipeId}";

        return await _factory.WithRetryAsync(async client =>
        {
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            return ParseRecipe(doc.RootElement);
        }, ct);
    }

    /// <summary>
    /// Get item metadata by ID.
    /// </summary>
    public async Task<ItemResponse?> GetItemAsync(uint itemId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/1/sheet/Item/{itemId}";

        return await _factory.WithRetryAsync(async client =>
        {
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            return ParseItem(doc.RootElement);
        }, ct);
    }

    /// <summary>
    /// Search for recipes that produce a given item.
    /// </summary>
    public async Task<IReadOnlyList<RecipeResponse>> GetRecipesForItemAsync(uint itemId, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/1/search?sheets=Recipe&query=ItemResult.value={itemId}";
        var results = new List<RecipeResponse>();

        var data = await _factory.WithRetryAsync(async client =>
        {
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }, ct);

        if (data is null) return results;

        try
        {
            var doc = JsonDocument.Parse(data);
            if (doc.RootElement.TryGetProperty("results", out var arr))
            {
                foreach (var el in arr.EnumerateArray())
                {
                    var recipe = ParseRecipe(el);
                    if (recipe is not null) results.Add(recipe);
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Warning($"[QM] XIVAPI recipe search parse error: {ex.Message}");
        }

        return results;
    }

    private static RecipeResponse? ParseRecipe(JsonElement el)
    {
        try
        {
            var recipe = new RecipeResponse
            {
                RecipeId = el.GetProperty("row_id").GetUInt32(),
            };

            if (el.TryGetProperty("fields", out var fields))
            {
                if (fields.TryGetProperty("ItemResult", out var itemResult) &&
                    itemResult.TryGetProperty("value", out var irVal))
                    recipe.ResultItemId = irVal.GetUInt32();

                if (fields.TryGetProperty("AmountResult", out var amountResult))
                    recipe.ResultQuantity = amountResult.GetInt32();

                var ingredients = new List<RecipeIngredient>();
                for (int i = 0; i < 10; i++)
                {
                    if (fields.TryGetProperty($"ItemIngredient[{i}]", out var ing) &&
                        ing.TryGetProperty("value", out var ingId) && ingId.GetUInt32() > 0 &&
                        fields.TryGetProperty($"AmountIngredient[{i}]", out var amt))
                    {
                        ingredients.Add(new RecipeIngredient
                        {
                            ItemId = ingId.GetUInt32(),
                            Quantity = amt.GetInt32(),
                        });
                    }
                }
                recipe.Ingredients = ingredients;
            }

            return recipe;
        }
        catch { return null; }
    }

    private static ItemResponse? ParseItem(JsonElement el)
    {
        try
        {
            var item = new ItemResponse();

            if (el.TryGetProperty("row_id", out var rowId))
                item.ItemId = rowId.GetUInt32();

            if (el.TryGetProperty("fields", out var fields))
            {
                if (fields.TryGetProperty("Name", out var name))
                    item.Name = name.GetString() ?? string.Empty;

                if (fields.TryGetProperty("Icon", out var icon) && icon.TryGetProperty("id", out var iconId))
                    item.IconId = iconId.GetInt32();

                if (fields.TryGetProperty("CanBeHq", out var hq))
                    item.CanBeCrafted = hq.GetBoolean(); // approximation

                if (fields.TryGetProperty("PriceMid", out var vendorPrice))
                {
                    var price = vendorPrice.GetInt32();
                    item.VendorPrice = price > 0 ? price : null;
                }
            }

            return item;
        }
        catch { return null; }
    }
}

// ─── XIVAPI Response DTOs ────────────────────────────────────────────────────

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

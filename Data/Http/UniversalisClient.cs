using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quartermaster.Data.Http;

/// <summary>
/// Market price polling client for the Universalis API.
/// https://universalis.app/docs/
/// </summary>
public class UniversalisClient
{
    private const string BaseUrl = "https://universalis.app/api/v2";
    private readonly HttpClientFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public UniversalisClient(HttpClientFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// GET /api/v2/{worldOrDc}/{itemIds}
    /// Fetches current listings for one or more items on a world.
    /// </summary>
    public async Task<MarketResponse?> GetListingsAsync(uint worldId, IEnumerable<uint> itemIds, CancellationToken ct = default)
    {
        var ids = string.Join(",", itemIds);
        var url = $"{BaseUrl}/{worldId}/{ids}?listings=20&entries=5&noGst=true";

        return await _factory.WithRetryAsync(async client =>
        {
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<MarketResponse>(json, JsonOpts);
        }, ct);
    }

    /// <summary>
    /// GET /api/v2/history/{worldOrDc}/{itemId}
    /// Fetches recent sale history for velocity calculations.
    /// </summary>
    public async Task<SaleHistoryResponse?> GetSaleHistoryAsync(uint worldId, uint itemId, int entries = 50, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/history/{worldId}/{itemId}?entries={entries}";

        return await _factory.WithRetryAsync(async client =>
        {
            var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<SaleHistoryResponse>(json, JsonOpts);
        }, ct);
    }

    /// <summary>
    /// Batch request — respects Universalis 100-item cap per request.
    /// Splits into multiple requests if needed.
    /// </summary>
    public async Task<IReadOnlyDictionary<uint, MarketResponse>> GetBatchAsync(uint worldId, IEnumerable<uint> itemIds, CancellationToken ct = default)
    {
        var result = new Dictionary<uint, MarketResponse>();
        var batches = itemIds.Chunk(100);

        foreach (var batch in batches)
        {
            ct.ThrowIfCancellationRequested();
            var response = await GetListingsAsync(worldId, batch, ct);
            if (response is not null)
            {
                result[response.ItemId] = response;
            }
        }

        return result;
    }
}

// ─── Universalis Response DTOs ───────────────────────────────────────────────

/// <summary>
/// Maps to Universalis /api/v2/{world}/{itemId} response.
/// </summary>
public class MarketResponse
{
    [JsonPropertyName("itemID")]
    public uint ItemId { get; set; }

    [JsonPropertyName("minPrice")]
    public int MinPrice { get; set; }

    [JsonPropertyName("minPriceNQ")]
    public int MinPriceNq { get; set; }

    [JsonPropertyName("minPriceHQ")]
    public int MinPriceHq { get; set; }

    [JsonPropertyName("averagePrice")]
    public float AvgPrice { get; set; }

    [JsonPropertyName("averagePriceNQ")]
    public float AvgPriceNq { get; set; }

    [JsonPropertyName("averagePriceHQ")]
    public float AvgPriceHq { get; set; }

    [JsonPropertyName("currentAveragePrice")]
    public float CurrentAvgPrice { get; set; }

    [JsonPropertyName("regularSaleVelocity")]
    public float SaleVelocity { get; set; }

    [JsonPropertyName("nqSaleVelocity")]
    public float NqSaleVelocity { get; set; }

    [JsonPropertyName("hqSaleVelocity")]
    public float HqSaleVelocity { get; set; }

    [JsonPropertyName("listingsCount")]
    public int ListingCount { get; set; }

    [JsonPropertyName("listings")]
    public List<MarketListing> Listings { get; set; } = new();

    [JsonPropertyName("recentHistory")]
    public List<SaleEntry> RecentHistory { get; set; } = new();

    [JsonPropertyName("lastUploadTime")]
    public long LastUploadTime { get; set; }
}

public class MarketListing
{
    [JsonPropertyName("pricePerUnit")]
    public int PricePerUnit { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("hq")]
    public bool IsHq { get; set; }

    [JsonPropertyName("retainerName")]
    public string RetainerName { get; set; } = string.Empty;

    [JsonPropertyName("lastReviewTime")]
    public long LastReviewTime { get; set; }
}

/// <summary>
/// Maps to Universalis /api/v2/history/{world}/{itemId} response.
/// </summary>
public class SaleHistoryResponse
{
    [JsonPropertyName("itemID")]
    public uint ItemId { get; set; }

    [JsonPropertyName("entries")]
    public List<SaleEntry> Entries { get; set; } = new();
}

public class SaleEntry
{
    [JsonPropertyName("pricePerUnit")]
    public int Price { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("hq")]
    public bool IsHq { get; set; }

    [JsonPropertyName("timestamp")]
    public long TimestampUnix { get; set; }

    [JsonIgnore]
    public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeSeconds(TimestampUnix);
}

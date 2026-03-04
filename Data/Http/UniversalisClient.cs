namespace Quartermaster.Data.Http;

/// <summary>
/// Market price polling client for the Universalis API.
/// </summary>
public class UniversalisClient
{
    private readonly HttpClient _httpClient;

    public UniversalisClient(HttpClientFactory factory)
    {
        _httpClient = factory.GetClient();
    }

    /// <summary>
    /// GET /api/v2/{worldOrDc}/{itemIds}
    /// </summary>
    public Task<MarketResponse?> GetListingsAsync(uint worldId, IEnumerable<uint> itemIds)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// GET /api/v2/history/{worldOrDc}/{itemId}
    /// </summary>
    public Task<SaleHistoryResponse?> GetSaleHistoryAsync(uint worldId, uint itemId, int entries = 50)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Batch request — respects Universalis 100-item cap per request.
    /// </summary>
    public Task<IReadOnlyDictionary<uint, MarketResponse>> GetBatchAsync(uint worldId, IEnumerable<uint> itemIds)
    {
        throw new NotImplementedException();
    }
}

// Response DTOs — stub types to be fleshed out in implementation
public class MarketResponse
{
    public uint ItemId { get; set; }
    public int MinPrice { get; set; }
    public int AvgPrice { get; set; }
    public int ListingCount { get; set; }
}

public class SaleHistoryResponse
{
    public uint ItemId { get; set; }
    public IReadOnlyList<SaleEntry> Entries { get; set; } = Array.Empty<SaleEntry>();
}

public class SaleEntry
{
    public int Price { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

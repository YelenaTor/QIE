namespace Quartermaster.Data.Http;

/// <summary>
/// Shared HttpClient management.
/// </summary>
public class HttpClientFactory
{
    private readonly HttpClient _client;

    public HttpClientFactory()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "Quartermaster-FFXIV-Plugin");
    }

    public HttpClient GetClient() => _client;

    public void Dispose()
    {
        _client.Dispose();
    }
}

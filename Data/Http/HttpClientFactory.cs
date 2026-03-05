namespace Quartermaster.Data.Http;

/// <summary>
/// Shared HttpClient management with retry and timeout.
/// </summary>
public class HttpClientFactory : IDisposable
{
    private readonly HttpClient _client;

    public HttpClientFactory()
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            MaxConnectionsPerServer = 4,
        };

        _client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(10),
        };
        _client.DefaultRequestHeaders.Add("User-Agent", "Quartermaster-FFXIV-Plugin/0.4.0");
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public HttpClient GetClient() => _client;

    /// <summary>
    /// Retry wrapper — retries up to 2 times with exponential backoff on transient failures.
    /// </summary>
    public async Task<T?> WithRetryAsync<T>(Func<HttpClient, Task<T?>> action, CancellationToken ct = default)
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                return await action(_client);
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested && attempt < 2)
            {
                Plugin.Log.Warning($"[QM] HTTP request timed out (attempt {attempt + 1}/3).");
                await Task.Delay(TimeSpan.FromMilliseconds(500 * (attempt + 1)), ct);
            }
            catch (OperationCanceledException) { throw; }
            catch (HttpRequestException ex) when (attempt < 2)
            {
                Plugin.Log.Warning($"[QM] HTTP request failed (attempt {attempt + 1}/3): {ex.Message}");
                await Task.Delay(TimeSpan.FromMilliseconds(500 * (attempt + 1)), ct);
            }
        }
        return default;
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}

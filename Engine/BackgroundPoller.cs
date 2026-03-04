namespace Quartermaster.Engine;

/// <summary>
/// Timed background worker wrapping PeriodicTimer with Dalamud-aware cancellation.
/// Respects player state — does not poll during loading screens or duty content unless configured.
/// </summary>
public class BackgroundPoller : IDisposable
{
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    private Task? _runningTask;

    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    public bool PauseInDuty { get; set; } = true;

    public Func<CancellationToken, Task>? OnTick { get; set; }

    public Task StartAsync()
    {
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(Interval);
        _runningTask = RunAsync(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task RunAsync(CancellationToken ct)
    {
        while (await _timer!.WaitForNextTickAsync(ct))
        {
            if (OnTick is not null)
            {
                await OnTick(ct);
            }
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
    }

    public void Dispose()
    {
        Stop();
        _timer?.Dispose();
        _cts?.Dispose();
    }
}

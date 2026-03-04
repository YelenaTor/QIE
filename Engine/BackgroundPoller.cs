using Dalamud.Game;
using Quartermaster.Configuration;

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
    public bool PauseInCombat { get; set; }

    public Func<CancellationToken, Task>? OnTick { get; set; }

    /// <summary>
    /// Configure from plugin settings.
    /// </summary>
    public void Configure(PluginConfig config)
    {
        Interval = config.PollInterval;
        PauseInDuty = config.PausePollInDuty;
        PauseInCombat = config.PausePollInCombat;
    }

    public Task StartAsync()
    {
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(Interval);
        _runningTask = RunAsync(_cts.Token);

        Plugin.Log.Information($"[QM] BackgroundPoller started (interval: {Interval.TotalMinutes:F0}m, pauseInDuty: {PauseInDuty}).");
        return Task.CompletedTask;
    }

    private async Task RunAsync(CancellationToken ct)
    {
        while (await _timer!.WaitForNextTickAsync(ct))
        {
            try
            {
                // Check duty/combat conditions
                if (PauseInDuty && Plugin.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty])
                {
                    Plugin.Log.Debug("[QM] Poller skipping — in duty.");
                    continue;
                }

                if (PauseInCombat && Plugin.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat])
                {
                    Plugin.Log.Debug("[QM] Poller skipping — in combat.");
                    continue;
                }

                if (OnTick is not null)
                {
                    await OnTick(ct);
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex, "[QM] BackgroundPoller tick error.");
            }
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
        Plugin.Log.Information("[QM] BackgroundPoller stopped.");
    }

    public void Dispose()
    {
        Stop();
        _timer?.Dispose();
        _cts?.Dispose();
    }
}

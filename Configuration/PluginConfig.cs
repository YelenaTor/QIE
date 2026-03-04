using Dalamud.Configuration;
using Quartermaster.Domain.Enums;

namespace Quartermaster.Configuration;

public class PluginConfig : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    // UI
    public AnchorSide AnchorSide { get; set; } = AnchorSide.Left;
    public float BarOpacity { get; set; } = 0.92f;
    public string? LastOpenDrawer { get; set; }

    // Engine
    public uint HomeWorldId { get; set; }
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(5);
    public bool PausePollInDuty { get; set; } = true;
    public bool PausePollInCombat { get; set; } = false;

    // Alert preferences
    public bool AlertsViaNotificationMaster { get; set; } = true;
    public bool AlertsViaToast { get; set; } = true;
    public bool AlertsViaChat { get; set; } = false;
    public AlertSeverity MinAlertSeverity { get; set; } = AlertSeverity.Info;

    // IPC behaviour
    public bool RequireConfirmBeforeCraft { get; set; } = true;
    public bool AutoSuppressRetainerDuringCraft { get; set; } = true;

    // Submarine & retainer
    public float SubmarineAlertThresholdGilPerHour { get; set; } = 50_000;
    public bool SubmarineAutoSuggest { get; set; } = true;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}

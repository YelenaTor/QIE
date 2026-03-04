using Quartermaster.Domain.Enums;

namespace Quartermaster.Configuration;

/// <summary>
/// Alert delivery preferences.
/// </summary>
public class AlertConfig
{
    public bool ViaNotificationMaster { get; set; } = true;
    public bool ViaToast { get; set; } = true;
    public bool ViaChat { get; set; } = false;
    public AlertSeverity MinSeverity { get; set; } = AlertSeverity.Info;
}

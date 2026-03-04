using Quartermaster.Domain.Enums;

namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// Toast/notification delivery via NotificationMaster IPC.
/// </summary>
public class NotificationMasterCapability
{
    public void SendToast(string title, string body, AlertSeverity severity)
    {
        throw new NotImplementedException();
    }

    public void SendChatMessage(string message)
    {
        throw new NotImplementedException();
    }
}

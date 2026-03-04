using Quartermaster.Domain.Enums;
using Una.Drawing;

namespace Quartermaster.UI.Components;

/// <summary>
/// IPC plugin connection status indicator. Builds status rows for the settings panel.
/// </summary>
public static class IpcStatusIndicator
{
    /// <summary>
    /// Build a single IPC status row.
    /// </summary>
    public static Node Build(string pluginName, bool isConnected)
    {
        var row = new Node { ClassList = { "ipc-status-row" } };

        var dot = new Node { ClassList = { "ipc-status-dot", isConnected ? "connected" : "missing" } };

        var name = new Node { ClassList = { "ipc-plugin-name", "qm-text" } };
        name.NodeValue = pluginName;

        var status = new Node { ClassList = { "ipc-status-label", isConnected ? "connected" : "missing" } };
        status.NodeValue = isConnected ? "Connected" : "Not detected";

        row.AppendChild(dot);
        row.AppendChild(name);
        row.AppendChild(status);

        return row;
    }
}

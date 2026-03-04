using Quartermaster.Configuration;
using Quartermaster.Domain.Enums;
using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Plugin configuration — anchor side, polling, alerts, IPC status.
/// </summary>
public class SettingsPanel : IDrawerPanel
{
    public string Id => "settings";
    public string Title => "Settings";
    public string Icon => "\uF013"; // FontAwesome cog
    public string Tooltip => "Settings — configure Quartermaster";
    public bool HasUnreadBadge => false;

    private Node? _rootNode;

    public Node BuildContent()
    {
        var doc = QuartermasterDrawing.LoadDocument("settings-panel.xml");
        _rootNode = doc.RootNode!;

        var config = Plugin.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();

        // Set section headers
        var sectionHeaders = _rootNode.QuerySelectorAll(".settings-section-header");
        if (sectionHeaders.Count >= 4)
        {
            sectionHeaders[0].NodeValue = "GENERAL";
            sectionHeaders[1].NodeValue = "POLLING";
            sectionHeaders[2].NodeValue = "ALERTS";
            sectionHeaders[3].NodeValue = "IPC STATUS";
        }

        // Set setting labels
        var labels = _rootNode.QuerySelectorAll(".settings-label");
        if (labels.Count >= 7)
        {
            labels[0].NodeValue = "Anchor Side";
            labels[1].NodeValue = "Home World";
            labels[2].NodeValue = "Poll Interval";
            labels[3].NodeValue = "Pause in Duty";
            labels[4].NodeValue = "Toast Notifications";
            labels[5].NodeValue = "Chat Notifications";
            labels[6].NodeValue = "Minimum Severity";
        }

        // Populate current values
        SetNodeText("AnchorSideValue", config.AnchorSide.ToString());
        SetNodeText("HomeWorldValue", config.HomeWorldId == 0 ? "Not set" : $"World {config.HomeWorldId}");
        SetNodeText("PollIntervalValue", $"{config.PollInterval.TotalMinutes:F0} min");
        SetNodeText("PauseDutyValue", config.PausePollInDuty ? "Yes" : "No");
        SetNodeText("AlertToastValue", config.AlertsViaToast ? "On" : "Off");
        SetNodeText("AlertChatValue", config.AlertsViaChat ? "On" : "Off");
        SetNodeText("MinSeverityValue", config.MinAlertSeverity.ToString());

        // Anchor side toggle
        var anchorNode = _rootNode.FindById("AnchorSideValue");
        if (anchorNode is not null)
        {
            anchorNode.OnMouseUp += (_, _) =>
            {
                config.AnchorSide = config.AnchorSide == AnchorSide.Left ? AnchorSide.Right : AnchorSide.Left;
                config.Save();
                anchorNode.NodeValue = config.AnchorSide.ToString();
            };
        }

        // Pause in duty toggle
        var pauseNode = _rootNode.FindById("PauseDutyValue");
        if (pauseNode is not null)
        {
            pauseNode.OnMouseUp += (_, _) =>
            {
                config.PausePollInDuty = !config.PausePollInDuty;
                config.Save();
                pauseNode.NodeValue = config.PausePollInDuty ? "Yes" : "No";
            };
        }

        // Toast toggle
        var toastNode = _rootNode.FindById("AlertToastValue");
        if (toastNode is not null)
        {
            toastNode.OnMouseUp += (_, _) =>
            {
                config.AlertsViaToast = !config.AlertsViaToast;
                config.Save();
                toastNode.NodeValue = config.AlertsViaToast ? "On" : "Off";
            };
        }

        // Chat toggle
        var chatNode = _rootNode.FindById("AlertChatValue");
        if (chatNode is not null)
        {
            chatNode.OnMouseUp += (_, _) =>
            {
                config.AlertsViaChat = !config.AlertsViaChat;
                config.Save();
                chatNode.NodeValue = config.AlertsViaChat ? "On" : "Off";
            };
        }

        // Build IPC status rows
        BuildIpcStatusRows();

        return _rootNode;
    }

    private void BuildIpcStatusRows()
    {
        var ipcList = _rootNode?.FindById("IpcStatusList");
        if (ipcList is null) return;

        var plugins = new[]
        {
            ("Artisan", IpcCapability.Artisan),
            ("AutoRetainer", IpcCapability.AutoRetainer),
            ("GatherBuddy", IpcCapability.GatherBuddy),
            ("AllaganTools", IpcCapability.AllaganTools),
            ("ItemVendorLocation", IpcCapability.ItemVendorLocation),
            ("NotificationMaster", IpcCapability.NotificationMaster),
        };

        foreach (var (name, _) in plugins)
        {
            var row = new Node { ClassList = { "ipc-status-row" } };

            var dot = new Node { ClassList = { "ipc-status-dot", "missing" } };
            var label = new Node { ClassList = { "ipc-plugin-name", "qm-text" } };
            label.NodeValue = name;
            var status = new Node { ClassList = { "ipc-status-label", "missing" } };
            status.NodeValue = "Not detected";

            row.AppendChild(dot);
            row.AppendChild(label);
            row.AppendChild(status);
            ipcList.AppendChild(row);
        }
    }

    private void SetNodeText(string id, string text)
    {
        var node = _rootNode?.FindById(id);
        if (node is not null) node.NodeValue = text;
    }
}

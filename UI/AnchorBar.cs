using Quartermaster.Configuration;
using Quartermaster.Domain.Enums;
using Quartermaster.UI.Panels;
using ImGuiNET;
using Una.Drawing;

namespace Quartermaster.UI;

/// <summary>
/// 48px wide, vertically centered, rounded on the outside edge only.
/// Renders as a Node tree via Una.Drawing on the background draw list.
/// Clicking an icon toggles the drawer. Clicking the active icon collapses it.
/// </summary>
public class AnchorBar
{
    private readonly Node _rootNode;
    private readonly Node _iconsNode;
    private readonly List<ToolbarEntry> _entries = new();
    private readonly DrawerManager _drawerManager;

    private string? _activeId;

    public AnchorBar(DrawerManager drawerManager)
    {
        _drawerManager = drawerManager;

        var doc = QuartermasterDrawing.LoadDocument("anchor-bar.xml");
        _rootNode = doc.RootNode!;
        _iconsNode = _rootNode.FindById("Icons")!;
    }

    /// <summary>
    /// Register a panel and create its icon node in the anchor bar.
    /// </summary>
    public void RegisterPanel(IDrawerPanel panel)
    {
        var entry = new ToolbarEntry(panel.Id, panel.Icon, panel.Tooltip, panel);
        _entries.Add(entry);

        var iconNode = new Node
        {
            Id = $"icon-{panel.Id}",
            ClassList = { "anchor-icon" },
            InheritTags = true,
        };
        iconNode.Style.FontSize = 16;

        // Click handler: toggle the drawer.
        iconNode.OnMouseUp += _ =>
        {
            if (_activeId == panel.Id)
            {
                _activeId = null;
                _drawerManager.Close();
            }
            else
            {
                _activeId = panel.Id;
                _drawerManager.Toggle(panel.Id);
            }
            UpdateActiveStates();
        };

        _iconsNode.AppendChild(iconNode);
    }

    /// <summary>
    /// Update anchor side from config.
    /// </summary>
    public void SetSide(AnchorSide side)
    {
        _rootNode.ToggleClass("left", side == AnchorSide.Left);
        _rootNode.ToggleClass("right", side == AnchorSide.Right);
    }

    /// <summary>
    /// Render the anchor bar to the background draw list.
    /// </summary>
    public void Draw()
    {
        UpdateBadges();

        var viewport = ImGui.GetMainViewport();
        var drawList = ImGui.GetBackgroundDrawList(viewport);

        float xPos = _rootNode.ClassList.Contains("right")
            ? viewport.WorkPos.X + viewport.WorkSize.X - 48
            : viewport.WorkPos.X;

        float yPos = viewport.WorkPos.Y + (viewport.WorkSize.Y * 0.15f);

        _rootNode.Style.Size = new Size(48, (int)(viewport.WorkSize.Y * 0.7f));
        _rootNode.Render(drawList, new System.Numerics.Vector2(xPos, yPos));
    }

    private void UpdateActiveStates()
    {
        foreach (var entry in _entries)
        {
            var iconNode = _iconsNode.FindById($"icon-{entry.Id}");
            iconNode?.ToggleClass("active", entry.Id == _activeId);
        }
    }

    private void UpdateBadges()
    {
        foreach (var entry in _entries)
        {
            var iconNode = _iconsNode.FindById($"icon-{entry.Id}");
            iconNode?.ToggleClass("has-badge", entry.Panel.HasUnreadBadge);
        }
    }
}

using Quartermaster.UI.Panels;
using ImGuiNET;
using Una.Drawing;

namespace Quartermaster.UI;

/// <summary>
/// Controls which panel is open, manages the drawer window lifecycle.
/// Drawer window: flush against bar, 380px wide, 70% screen height.
/// Uses Una.Drawing Node tree from drawer.xml template.
/// Calls active panel's BuildContent() to populate the drawer body.
/// </summary>
public class DrawerManager
{
    private string? _activeId;
    private readonly Dictionary<string, ToolbarEntry> _entries = new();
    private readonly Node _rootNode;
    private readonly Node _titleNode;
    private readonly Node _bodyNode;
    private readonly Node _footerNode;
    private readonly Node _closeNode;

    public DrawerManager()
    {
        var doc = QuartermasterDrawing.LoadDocument("drawer.xml");
        _rootNode = doc.RootNode!;
        _titleNode = _rootNode.FindById("DrawerTitle")!;
        _bodyNode = _rootNode.FindById("DrawerBody")!;
        _footerNode = _rootNode.FindById("DrawerFooter")!;
        _closeNode = _rootNode.FindById("DrawerClose")!;

        _closeNode.NodeValue = "\u2715"; // ✕
        _closeNode.OnMouseUp += _ => Close();
    }

    public void RegisterPanel(IDrawerPanel panel)
    {
        _entries[panel.Id] = new ToolbarEntry(panel.Id, panel.Icon, panel.Tooltip, panel);
    }

    public void Toggle(string drawerId)
    {
        if (_activeId == drawerId)
        {
            Close();
            return;
        }

        _activeId = drawerId;
        RebuildContent();
    }

    public void Close()
    {
        _activeId = null;
        ClearBody();
    }

    public bool IsOpen => _activeId is not null;

    /// <summary>
    /// Called each framework draw frame.
    /// </summary>
    public void Draw()
    {
        if (_activeId is null || !_entries.TryGetValue(_activeId, out _))
            return;

        var viewport = ImGui.GetMainViewport();
        var drawList = ImGui.GetBackgroundDrawList(viewport);

        // Position flush against anchor bar.
        float drawerHeight = viewport.WorkSize.Y * 0.7f;
        float yPos = viewport.WorkPos.Y + (viewport.WorkSize.Y * 0.15f);

        // TODO: Support left-side anchor (currently right-side only).
        float xPos = viewport.WorkPos.X + viewport.WorkSize.X - 48 - 380;

        _rootNode.Style.Size = new Size(380, (int)drawerHeight);
        _rootNode.Render(drawList, new System.Numerics.Vector2(xPos, yPos));
    }

    /// <summary>
    /// Rebuild the drawer body with the active panel's content.
    /// Call this when data changes and the panel needs to refresh.
    /// </summary>
    public void RebuildContent()
    {
        if (_activeId is null || !_entries.TryGetValue(_activeId, out var entry))
            return;

        _titleNode.NodeValue = entry.Panel.Title;

        ClearBody();

        var panelContent = entry.Panel.BuildContent();
        _bodyNode.AppendChild(panelContent);
    }

    private void ClearBody()
    {
        _bodyNode.ChildNodes.ToList().ForEach(c => _bodyNode.RemoveChild(c));
    }
}

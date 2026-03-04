using Quartermaster.UI.Panels;
using Una.Drawing;

namespace Quartermaster.UI;

/// <summary>
/// Controls which panel is open, manages the drawer window lifecycle.
/// Drawer window: flush against bar, 380px wide, 70% screen height.
/// Uses Una.Drawing Node tree from drawer.xml template.
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

        _closeNode.OnMouseUp += (_, _) => Close();
    }

    public void RegisterPanel(IDrawerPanel panel)
    {
        _entries[panel.Id] = new ToolbarEntry(panel.Id, panel.Icon, panel.Tooltip, panel);
    }

    public void Toggle(string drawerId)
    {
        _activeId = _activeId == drawerId ? null : drawerId;
        UpdateContent();
    }

    public void Close()
    {
        _activeId = null;
    }

    public bool IsOpen => _activeId is not null;

    /// <summary>
    /// Called each framework draw frame.
    /// </summary>
    public void Draw()
    {
        if (_activeId is null || !_entries.TryGetValue(_activeId, out var entry))
            return;

        var viewport = ImGuiNET.ImGui.GetMainViewport();
        var drawList = ImGuiNET.ImGui.GetBackgroundDrawList(viewport);

        // Position flush against anchor bar.
        float drawerHeight = viewport.WorkSize.Y * 0.7f;
        float yPos = viewport.WorkPos.Y + (viewport.WorkSize.Y * 0.15f);

        // TODO: Support left-side anchor (currently right-side only).
        float xPos = viewport.WorkPos.X + viewport.WorkSize.X - 48 - 380;

        _rootNode.Style.Size = new(380, (int)drawerHeight);
        _rootNode.Render(drawList, new(xPos, yPos));
    }

    private void UpdateContent()
    {
        if (_activeId is not null && _entries.TryGetValue(_activeId, out var entry))
        {
            _titleNode.NodeValue = entry.Panel.Title;
        }
    }
}

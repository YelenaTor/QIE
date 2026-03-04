using Quartermaster.UI.Panels;

namespace Quartermaster.UI;

/// <summary>
/// Controls which panel is open, manages the drawer window lifecycle.
/// Drawer window: flush against bar, 380px wide, 70% screen height.
/// </summary>
public class DrawerManager
{
    private string? _activeId;
    private readonly List<ToolbarEntry> _entries = new();

    public void RegisterPanel(IDrawerPanel panel)
    {
        _entries.Add(new ToolbarEntry(panel.Id, panel.Icon, panel.Tooltip, panel));
    }

    public void Toggle(string drawerId)
    {
        _activeId = _activeId == drawerId ? null : drawerId;
    }

    public void Close()
    {
        _activeId = null;
    }

    /// <summary>
    /// Called each framework update frame.
    /// </summary>
    public void Draw()
    {
        if (_activeId is null) return;

        var entry = _entries.Find(e => e.Id == _activeId);
        if (entry is not null)
        {
            DrawPanel(entry);
        }
    }

    private void DrawPanel(ToolbarEntry entry)
    {
        // TODO: Create flush ImGui window, 380px wide, 70% screen height
        entry.Panel.Draw();
    }
}

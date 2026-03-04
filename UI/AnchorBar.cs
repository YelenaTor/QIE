using Quartermaster.UI.Panels;

namespace Quartermaster.UI;

/// <summary>
/// 48px wide, vertically centered, rounded on the outside edge only.
/// Renders as a transparent ImGui window with a custom-drawn background.
/// Clicking an icon toggles the drawer. Clicking the active icon collapses it.
/// </summary>
public class AnchorBar
{
    private readonly List<ToolbarEntry> _entries = new();

    public void RegisterPanel(IDrawerPanel panel, Action<string> onToggle)
    {
        _entries.Add(new ToolbarEntry(panel.Id, panel.Icon, panel.Tooltip, panel));
    }

    public void Draw()
    {
        // TODO: Render 48px sidebar with icons
        // TODO: Gold pip on active panel
        // TODO: Red badge count on alerts icon
    }
}

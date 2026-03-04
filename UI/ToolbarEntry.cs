using Quartermaster.UI.Panels;

namespace Quartermaster.UI;

/// <summary>
/// Model: id, icon, tooltip, panel ref.
/// </summary>
public record ToolbarEntry(
    string Id,
    string Icon,
    string Tooltip,
    IDrawerPanel Panel
);

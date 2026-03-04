using Quartermaster.Domain.Enums;

namespace Quartermaster.Configuration;

/// <summary>
/// Anchor side, open drawer, and other UI preferences.
/// </summary>
public class UIConfig
{
    public AnchorSide AnchorSide { get; set; } = AnchorSide.Left;
    public float BarOpacity { get; set; } = 0.92f;
    public string? LastOpenDrawer { get; set; }
}

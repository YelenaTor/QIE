namespace Quartermaster.UI.Panels;

/// <summary>
/// Sector ranking, voyage advisor. Ranked sector list by expected gil/hour.
/// </summary>
public class SubmarinePanel : IDrawerPanel
{
    public string Id => "submarines";
    public string Title => "Submarines";
    public string Icon => "\uF21A"; // FontAwesome ship
    public string Tooltip => "Submarines — voyage advisor";
    public bool HasUnreadBadge => false;

    public void Draw()
    {
        // TODO: Active voyages (timer, route, expected return)
        // TODO: Sector ranking table (name, avg gil/hr, voyage time, next recommended)
        // TODO: [Send Fleet] button (requires AutoRetainer IPC + user confirm)
    }
}

namespace Quartermaster.UI.Panels;

/// <summary>
/// Interface for all drawer panels. Each panel is a self-contained class
/// that receives services via constructor injection and renders purely via ImGui calls.
/// </summary>
public interface IDrawerPanel
{
    string Id { get; }
    string Title { get; }
    string Icon { get; }       // FontAwesome char
    string Tooltip { get; }
    bool HasUnreadBadge { get; }
    void Draw();
}

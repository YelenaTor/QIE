using Una.Drawing;

namespace Quartermaster.UI.Panels;

/// <summary>
/// Interface for all drawer panels. Each panel returns a Node subtree
/// that gets appended to the drawer body by DrawerManager.
/// </summary>
public interface IDrawerPanel
{
    string Id { get; }
    string Title { get; }
    string Icon { get; }       // FontAwesome char
    string Tooltip { get; }
    bool HasUnreadBadge { get; }

    /// <summary>
    /// Build and return the panel's content as a Node subtree.
    /// Called by DrawerManager when this panel is activated.
    /// </summary>
    Node BuildContent();
}

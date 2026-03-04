namespace Quartermaster.Util;

/// <summary>
/// Load game icons via Dalamud TextureProvider.
/// </summary>
public static class ItemIconHelper
{
    /// <summary>
    /// Get a texture handle for an item's icon.
    /// </summary>
    public static nint GetIconTextureHandle(int iconId)
    {
        // TODO: Use Services.TextureProvider.GetIcon(iconId)
        return nint.Zero;
    }
}

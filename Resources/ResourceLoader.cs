using System.Reflection;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;

namespace Quartermaster.Resources;

/// <summary>
/// Helper methods for loading embedded resources (icons, fonts, textures).
/// All assets under Resources/ are compiled as EmbeddedResource.
/// </summary>
public static class ResourceLoader
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// Get a raw resource stream by relative path (e.g. "Icons.icon.png").
    /// </summary>
    public static Stream? GetStream(string relativePath)
    {
        var fullName = $"Quartermaster.Resources.{relativePath}";
        return Assembly.GetManifestResourceStream(fullName);
    }

    /// <summary>
    /// Read an embedded resource as a byte array.
    /// </summary>
    public static byte[]? GetBytes(string relativePath)
    {
        using var stream = GetStream(relativePath);
        if (stream is null) return null;

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Load an embedded texture via Dalamud's TextureProvider.
    /// Usage: ResourceLoader.GetTexture("Textures.logo.png")
    /// </summary>
    public static ISharedImmediateTexture GetTexture(string relativePath)
    {
        return Plugin.TextureProvider.GetFromManifestResource(
            Assembly,
            $"Quartermaster.Resources.{relativePath}");
    }
}

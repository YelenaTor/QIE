using System.Reflection;
using Dalamud.Plugin;
using Una.Drawing;
using Una.Drawing.Clipping;

namespace Quartermaster.UI;

/// <summary>
/// Bridge between Quartermaster and Una.Drawing.
/// Initializes the drawing library, registers global stylesheets,
/// and loads UDT documents from embedded resources.
/// </summary>
internal static class QuartermasterDrawing
{
    public static void Initialize(IDalamudPluginInterface plugin)
    {
        DrawingLib.Setup(plugin);

        // Load and register the global stylesheet.
        var globals = LoadDocument("globals.xml");
        if (globals.Stylesheet is not null)
        {
            StylesheetRegistry.Register("qm-globals", globals.Stylesheet);
        }
    }

    public static void Dispose()
    {
        DrawingLib.Dispose();
    }

    /// <summary>
    /// Load a UDT document from embedded resources by filename.
    /// </summary>
    public static UdtDocument LoadDocument(string resourceName)
    {
        return UdtLoader.LoadFromAssembly(
            Assembly.GetExecutingAssembly(),
            resourceName
        );
    }

    /// <summary>
    /// Set image bytes on a node from an embedded resource.
    /// </summary>
    public static void SetImageFromResource(this Node node, string resourceName)
    {
        resourceName = resourceName.ToLowerInvariant();
        var asm = Assembly.GetExecutingAssembly();

        foreach (var name in asm.GetManifestResourceNames())
        {
            if (name.ToLowerInvariant().EndsWith(resourceName))
            {
                using var stream = asm.GetManifestResourceStream(name);
                if (stream == null) continue;

                var imageData = new byte[stream.Length];
                _ = stream.Read(imageData, 0, imageData.Length);

                node.Style.ImageBytes = imageData;
                return;
            }
        }
    }
}

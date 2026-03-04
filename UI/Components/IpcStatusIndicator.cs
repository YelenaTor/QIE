using Quartermaster.Domain.Enums;

namespace Quartermaster.UI.Components;

/// <summary>
/// Shows which plugins are detected with green/grey/red indicators.
/// </summary>
public class IpcStatusIndicator
{
    public void Draw(IpcCapability detectedCapabilities)
    {
        // TODO: For each known capability, show:
        //   Green dot + name if detected
        //   Grey dot + name if not detected
        //   Red dot + name if error
    }
}

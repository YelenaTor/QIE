namespace Quartermaster.UI;

/// <summary>
/// User approval popup for IPC actions. Modal — blocks the drawer until user responds.
/// All IPC-triggered actions go through this.
/// </summary>
public class ConfirmActionModal
{
    public bool IsOpen { get; private set; }

    private string _title = string.Empty;
    private string _description = string.Empty;
    private Action? _onConfirm;
    private Action? _onAdjust;
    private Action? _onCancel;

    /// <summary>
    /// Call this before any IPC action.
    /// </summary>
    public void Request(
        string title,
        string description,
        Action onConfirm,
        Action? onAdjust = null,
        Action? onCancel = null)
    {
        _title = title;
        _description = description;
        _onConfirm = onConfirm;
        _onAdjust = onAdjust;
        _onCancel = onCancel;
        IsOpen = true;
    }

    /// <summary>
    /// Renders the popup if open.
    /// </summary>
    public void Draw()
    {
        if (!IsOpen) return;

        // TODO: ImGui.BeginPopupModal
        // TODO: Title, description, Confirm/Adjust/Cancel buttons
        // TODO: Call appropriate callback and set IsOpen = false
    }
}

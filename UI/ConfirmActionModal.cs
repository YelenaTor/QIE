using Dalamud.Bindings.ImGui;
using Una.Drawing;

namespace Quartermaster.UI;

/// <summary>
/// User approval popup for IPC actions. Modal — blocks interaction until user responds.
/// All IPC-triggered actions go through this. Uses Una.Drawing node tree.
/// </summary>
public class ConfirmActionModal
{
    public bool IsOpen { get; private set; }

    private string _title = string.Empty;
    private string _description = string.Empty;
    private Action? _onConfirm;
    private Action? _onAdjust;
    private Action? _onCancel;

    private Node? _rootNode;

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

        BuildModal();
        IsOpen = true;
    }

    /// <summary>
    /// Renders the modal if open.
    /// </summary>
    public void Draw()
    {
        if (!IsOpen || _rootNode is null) return;

        var viewport = ImGui.GetMainViewport();
        var drawList = ImGui.GetBackgroundDrawList(viewport);

        _rootNode.Style.Size = new Size((int)viewport.WorkSize.X, (int)viewport.WorkSize.Y);
        _rootNode.Render(drawList, new System.Numerics.Vector2(viewport.WorkPos.X, viewport.WorkPos.Y));
    }

    private void BuildModal()
    {
        var doc = QuartermasterDrawing.LoadDocument("confirm-modal.xml");
        _rootNode = doc.RootNode!;

        // Populate text
        var titleNode = _rootNode.FindById("ConfirmTitle");
        if (titleNode is not null) titleNode.NodeValue = _title;

        var descNode = _rootNode.FindById("ConfirmDescription");
        if (descNode is not null) descNode.NodeValue = _description;

        // Wire button handlers
        var confirmBtn = _rootNode.FindById("ConfirmBtn");
        if (confirmBtn is not null)
        {
            confirmBtn.NodeValue = "Confirm";
            confirmBtn.OnMouseUp += _ => { _onConfirm?.Invoke(); Close(); };
        }

        var adjustBtn = _rootNode.FindById("AdjustBtn");
        if (adjustBtn is not null)
        {
            if (_onAdjust is not null)
            {
                adjustBtn.NodeValue = "Adjust";
                adjustBtn.OnMouseUp += _ => { _onAdjust?.Invoke(); Close(); };
            }
            else
            {
                adjustBtn.Style.IsVisible = false;
            }
        }

        var cancelBtn = _rootNode.FindById("CancelBtn");
        if (cancelBtn is not null)
        {
            cancelBtn.NodeValue = "Cancel";
            cancelBtn.OnMouseUp += _ => { _onCancel?.Invoke(); Close(); };
        }

        // Backdrop click = cancel
        var backdrop = _rootNode.QuerySelector(".confirm-backdrop");
        if (backdrop is not null)
        {
            backdrop.OnMouseUp += _ => { _onCancel?.Invoke(); Close(); };
        }
    }

    private void Close()
    {
        IsOpen = false;
        _rootNode = null;
    }
}

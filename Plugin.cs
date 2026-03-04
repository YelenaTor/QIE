using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.DalamudServices;
using Quartermaster.Configuration;
using Quartermaster.Data.Database;
using Quartermaster.Engine;
using Quartermaster.Ipc;
using Quartermaster.UI;

namespace Quartermaster;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    public string Name => "Quartermaster";

    public PluginConfig Configuration { get; init; }

    private readonly QuartermasterDb _db;
    private readonly IpcBridge _ipcBridge;
    private readonly WatchlistProcessor _engine;
    private readonly BackgroundPoller _poller;
    private readonly AnchorBar _anchorBar;
    private readonly DrawerManager _drawerManager;
    private readonly ConfirmActionModal _confirmModal;

    public Plugin()
    {
        ECommonsMain.Init(PluginInterface, this);

        Configuration = PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();

        // Initialize layers bottom-up
        _db = new QuartermasterDb();
        _ipcBridge = new IpcBridge();
        _engine = new WatchlistProcessor();
        _poller = new BackgroundPoller();
        _anchorBar = new AnchorBar();
        _drawerManager = new DrawerManager();
        _confirmModal = new ConfirmActionModal();

        Framework.Update += OnUpdate;
        PluginInterface.UiBuilder.Draw += OnDraw;

        Log.Information("[Quartermaster] Started successfully.");
    }

    private void OnUpdate(IFramework framework)
    {
        _ipcBridge.RefreshCapabilities();
    }

    private void OnDraw()
    {
        _anchorBar.Draw();
        _drawerManager.Draw();
        _confirmModal.Draw();
    }

    public void Dispose()
    {
        ECommonsMain.Dispose();

        Framework.Update -= OnUpdate;
        PluginInterface.UiBuilder.Draw -= OnDraw;

        _poller.Dispose();
        _db.Dispose();
    }
}

using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Quartermaster.Configuration;
using Quartermaster.Data.Cache;
using Quartermaster.Data.Database;
using Quartermaster.Data.Http;
using Quartermaster.Engine;
using Quartermaster.Ipc;
using Quartermaster.UI;
using Quartermaster.UI.Panels;

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

    // Data layer
    private readonly QuartermasterDb _db;
    private readonly HttpClientFactory _httpFactory;
    private readonly UniversalisClient _universalis;
    private readonly XivApiClient _xivApi;
    private readonly PriceCache _priceCache;
    private readonly RecipeCache _recipeCache;

    // Engine layer
    private readonly WatchlistProcessor _engine;
    private readonly BackgroundPoller _poller;

    // IPC layer
    private readonly IpcBridge _ipcBridge;

    // UI layer
    private readonly AnchorBar _anchorBar;
    private readonly DrawerManager _drawerManager;
    private readonly ConfirmActionModal _confirmModal;

    private bool _barVisible = true;

    public Plugin()
    {
        ECommonsMain.Init(PluginInterface, this);

        // Initialize Una.Drawing
        QuartermasterDrawing.Initialize(PluginInterface);

        Configuration = PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();

        // ─── Data Layer ─────────────────────────────────────────────────
        _db = new QuartermasterDb();
        _httpFactory = new HttpClientFactory();
        _universalis = new UniversalisClient(_httpFactory);
        _xivApi = new XivApiClient(_httpFactory);
        _priceCache = new PriceCache();
        _recipeCache = new RecipeCache();

        // Initialize DB asynchronously (fire-and-forget, errors handled internally)
        Task.Run(async () => await _db.InitializeAsync());

        // ─── Engine Layer ───────────────────────────────────────────────
        _engine = new WatchlistProcessor();
        _poller = new BackgroundPoller();
        _poller.Configure(Configuration);
        _poller.OnTick = _engine.ProcessAsync;

        // Start the poller
        _ = _poller.StartAsync();

        // ─── IPC Layer ──────────────────────────────────────────────────
        _ipcBridge = new IpcBridge();

        // ─── UI Layer ───────────────────────────────────────────────────
        _drawerManager = new DrawerManager();
        _anchorBar = new AnchorBar(_drawerManager);
        _confirmModal = new ConfirmActionModal();

        // Register all panels
        var panels = new IDrawerPanel[]
        {
            new WatchlistPanel(),
            new AlertPanel(),
            new MarketPanel(),
            new SubmarinePanel(),
            new RetainerPanel(),
            new SettingsPanel(),
        };

        foreach (var panel in panels)
        {
            _anchorBar.RegisterPanel(panel);
            _drawerManager.RegisterPanel(panel);
        }

        // Set anchor side from config
        _anchorBar.SetSide(Configuration.AnchorSide);

        // ─── Commands ───────────────────────────────────────────────────
        CommandManager.AddHandler("/qm", new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle the Quartermaster anchor bar.",
            ShowInHelp = true,
        });

        // ─── Events ─────────────────────────────────────────────────────
        Framework.Update += OnUpdate;
        PluginInterface.UiBuilder.Draw += OnDraw;

        Log.Information("[Quartermaster] v0.4.0 loaded successfully.");
    }

    private void OnCommand(string command, string args)
    {
        switch (args.ToLowerInvariant().Trim())
        {
            case "":
            case "toggle":
                _barVisible = !_barVisible;
                ChatGui.Print($"[QM] Anchor bar {(_barVisible ? "shown" : "hidden")}.");
                break;

            case "config":
            case "settings":
                _drawerManager.Toggle("settings");
                _barVisible = true;
                break;

            default:
                ChatGui.Print("[QM] Usage: /qm [toggle|config]");
                break;
        }
    }

    private void OnUpdate(IFramework framework)
    {
        _ipcBridge.RefreshCapabilities();
    }

    private void OnDraw()
    {
        if (_barVisible)
        {
            _anchorBar.Draw();
            _drawerManager.Draw();
        }

        _confirmModal.Draw();
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler("/qm");
        Framework.Update -= OnUpdate;
        PluginInterface.UiBuilder.Draw -= OnDraw;

        _poller.Dispose();
        _httpFactory.Dispose();
        _db.Dispose();

        QuartermasterDrawing.Dispose();
        ECommonsMain.Dispose();
    }
}

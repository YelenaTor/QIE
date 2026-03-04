# Quartermaster — Plugin Architecture & Design Specification

> **Version:** 0.1 (Pre-implementation)
> **Stack:** C# / .NET 8 / Dalamud API10+ / ImGui / SQLite
> **Author:** Klara
> **Scope:** FFXIV Economy Intelligence Engine — standalone Dalamud plugin with anchored sidebar UI and conditional IPC orchestration

---

## Table of Contents

1. [Goals & Non-Goals](#goals--non-goals)
2. [High-Level Architecture](#high-level-architecture)
3. [Project & File Layout](#project--file-layout)
4. [Layer Breakdown](#layer-breakdown)
   - [Domain (Core)](#1-domain-core)
   - [Data Layer](#2-data-layer)
   - [Intelligence Engine](#3-intelligence-engine)
   - [IPC Bridge](#4-ipc-bridge)
   - [UI Layer](#5-ui-layer)
   - [Plugin Entry & Infrastructure](#6-plugin-entry--infrastructure)
5. [Database Schema](#database-schema)
6. [IPC Integration Map](#ipc-integration-map)
7. [UI Structure](#ui-structure)
8. [Configuration Model](#configuration-model)
9. [Background Worker Model](#background-worker-model)
10. [Phased Delivery Plan](#phased-delivery-plan)
11. [Key Technical Decisions](#key-technical-decisions)
12. [Dependency Graph](#dependency-graph)

---

## Goals & Non-Goals

### Goals
- Consolidate market, crafting, retainer, and submarine profit data into one always-available dashboard
- Evaluate profitability continuously against a user-defined watchlist and emit actionable alerts
- Orchestrate actions across other Dalamud plugins (Artisan, AutoRetainer, GatherBuddyReborn) via IPC — always with user approval
- Present a non-intrusive anchored sidebar UI (Umbra-style) that the user can collapse at any time
- Degrade gracefully when optional plugins are absent — intelligence features always work standalone

### Non-Goals
- **Not** a replacement for Artisan, AutoRetainer, or GatherBuddyReborn — QM orchestrates, it does not re-implement
- **Not** a server-side product in v1 — telemetry/competition index is explicitly Phase 5+
- **Not** a real-time trading bot — all automation steps require explicit user confirmation
- **Not** dependent on Redis or Postgres — SQLite is the local store, HTTP is the external interface

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                        Quartermaster Plugin                          │
│                                                                       │
│  ┌─────────────────────────────────────────────────────────────┐    │
│  │                        UI Layer                              │    │
│  │   AnchorBar ──► DrawerManager ──► [Panel per feature]        │    │
│  └────────────────────────┬────────────────────────────────────┘    │
│                            │ reads/triggers                          │
│  ┌─────────────────────────▼──────────────────────────────────┐     │
│  │                   Intelligence Engine                        │     │
│  │   ProfitCalculator · AlertEvaluator · VelocityTracker        │     │
│  │   WatchlistProcessor · SubmarineAdvisor · RetainerAdvisor    │     │
│  └──────────┬─────────────────────────────────┬───────────────┘     │
│             │ reads                            │ emits               │
│  ┌──────────▼──────────┐           ┌───────────▼──────────────┐     │
│  │     Data Layer       │           │      IPC Bridge           │     │
│  │  Repositories        │           │  (conditional, optional)  │     │
│  │  SQLite via EF Core  │           │  Artisan · AutoRetainer   │     │
│  │  HTTP clients        │           │  GatherBuddyReborn        │     │
│  │  In-memory cache     │           │  AllaganTools · ItemVendor│     │
│  └──────────┬──────────┘           │  NotificationMaster       │     │
│             │                       └───────────────────────────┘     │
└─────────────┼───────────────────────────────────────────────────────┘
              │
   ┌──────────▼──────────────────┐
   │     External Services        │
   │  Universalis (HTTP / WS)     │
   │  XIVAPI (recipe/item data)   │
   └─────────────────────────────┘
```

---

## Project & File Layout

```
Quartermaster/
│
├── Quartermaster.csproj
├── Quartermaster.sln
│
├── Plugin.cs                          # IDalamudPlugin entry point
├── Services.cs                        # Static service locator / DI root
│
├── Configuration/
│   ├── PluginConfig.cs                # IPluginConfiguration root
│   ├── WatchlistConfig.cs             # Per-item watchlist threshold config
│   ├── AlertConfig.cs                 # Alert delivery preferences
│   ├── SubmarineConfig.cs             # Submarine module preferences
│   ├── RetainerConfig.cs              # Retainer module preferences
│   └── UIConfig.cs                    # Anchor side, open drawer, etc.
│
├── Domain/
│   ├── Models/
│   │   ├── WatchlistEntry.cs          # Item + threshold definition
│   │   ├── PriceSnapshot.cs           # Universalis price record
│   │   ├── RecipeGraph.cs             # Ingredient tree for an item
│   │   ├── CraftProfitResult.cs       # Output of ProfitCalculator
│   │   ├── AlertEvent.cs              # Fired when a watchlist item triggers
│   │   ├── SubmarineRoute.cs          # Sector + expected value
│   │   ├── VentureResult.cs           # Retainer venture record
│   │   └── CompetitionScore.cs        # Derived competition pressure (local)
│   │
│   └── Enums/
│       ├── AlertSeverity.cs           # Info / Warning / Opportunity / Urgent
│       ├── AnchorSide.cs              # Left / Right
│       ├── IpcCapability.cs           # Bitmask of detected plugins
│       └── MarketTrend.cs             # Rising / Stable / Falling / Volatile
│
├── Data/
│   ├── Database/
│   │   ├── QuartermasterDb.cs         # EF Core DbContext
│   │   ├── Migrations/                # EF migration files
│   │   └── Entities/
│   │       ├── ItemEntity.cs          # Cached item metadata
│   │       ├── WatchlistEntity.cs     # Persisted watchlist entries
│   │       ├── PriceHistoryEntity.cs  # Price snapshots over time
│   │       ├── AlertHistoryEntity.cs  # Fired alerts log
│   │       ├── RecipeEntity.cs        # Cached recipe graphs
│   │       ├── SubmarineEntity.cs     # Submarine/sector data
│   │       └── VentureHistoryEntity.cs
│   │
│   ├── Repositories/
│   │   ├── IWatchlistRepository.cs
│   │   ├── WatchlistRepository.cs
│   │   ├── IPriceRepository.cs
│   │   ├── PriceRepository.cs
│   │   ├── IRecipeRepository.cs
│   │   ├── RecipeRepository.cs
│   │   ├── IAlertRepository.cs
│   │   ├── AlertRepository.cs
│   │   ├── ISubmarineRepository.cs
│   │   └── SubmarineRepository.cs
│   │
│   ├── Http/
│   │   ├── UniversalisClient.cs       # Market price polling + WS (future)
│   │   ├── XivApiClient.cs            # Recipe graphs, item metadata
│   │   └── HttpClientFactory.cs       # Shared HttpClient management
│   │
│   └── Cache/
│       ├── PriceCache.cs              # TTL-based Dictionary<uint, PriceSnapshot>
│       └── RecipeCache.cs             # In-memory recipe graph cache
│
├── Engine/
│   ├── ProfitCalculator.cs            # Core profit margin computation
│   ├── VelocityTracker.cs             # Sale velocity from price history
│   ├── AlertEvaluator.cs              # Watchlist threshold evaluation
│   ├── WatchlistProcessor.cs          # Orchestrates polling → eval → alerts
│   ├── SubmarineAdvisor.cs            # Expected gil/hour per sector
│   ├── RetainerAdvisor.cs             # Venture profitability comparison
│   ├── CompetitionEstimator.cs        # Local listing-based competition score
│   └── BackgroundPoller.cs            # Timed background worker (Task-based)
│
├── Ipc/
│   ├── IpcBridge.cs                   # Capability detection + facade
│   ├── Capabilities/
│   │   ├── ArtisanCapability.cs       # Send craft list, check readiness
│   │   ├── AutoRetainerCapability.cs  # Read ventures, suppress, submarine state
│   │   ├── GatherBuddyCapability.cs   # Queue gather request
│   │   ├── AllaganToolsCapability.cs  # Inventory queries
│   │   ├── ItemVendorCapability.cs    # NPC price lookups
│   │   └── NotificationMasterCapability.cs  # Toast/notification delivery
│   └── IpcConstants.cs                # Plugin name strings, IPC method names
│
├── UI/
│   ├── AnchorBar.cs                   # Pinned sidebar icon strip
│   ├── DrawerManager.cs               # Controls which panel is open
│   ├── ToolbarEntry.cs                # Model: id, icon, tooltip, panel ref
│   │
│   ├── Panels/
│   │   ├── IDrawerPanel.cs            # Interface: Title, Draw()
│   │   ├── WatchlistPanel.cs          # Add/edit/remove watchlist items
│   │   ├── AlertPanel.cs              # Alert feed with action buttons
│   │   ├── MarketPanel.cs             # Item detail: price chart, cost breakdown
│   │   ├── SubmarinePanel.cs          # Sector ranking, voyage advisor
│   │   ├── RetainerPanel.cs           # Venture status, reassignment suggestions
│   │   └── SettingsPanel.cs           # Anchor side, thresholds, IPC status
│   │
│   └── Components/
│       ├── ProfitBadge.cs             # Reusable profit margin chip
│       ├── TrendArrow.cs              # Price trend indicator (↑ ↓ →)
│       ├── AlertCard.cs               # Single alert row with action buttons
│       ├── IngredientTree.cs          # Recursive recipe cost breakdown
│       ├── IpcStatusIndicator.cs      # Shows which plugins are detected
│       └── ConfirmActionModal.cs      # User approval popup for IPC actions
│
└── Util/
    ├── ItemIconHelper.cs              # Load game icons via Dalamud TextureProvider
    ├── GilFormatter.cs                # Format numbers: 1200 → "1.2k"
    ├── TimeFormatter.cs               # "3h 20m remaining" etc.
    ├── WorldHelper.cs                 # World/datacenter resolution
    └── PluginDetector.cs              # DalamudReflector plugin presence checks
```

---

## Layer Breakdown

### 1. Domain (Core)

Pure C# models and enums. **No Dalamud dependencies.** This layer is independently unit-testable.

```csharp
// Domain/Models/WatchlistEntry.cs
public record WatchlistEntry(
    uint ItemId,
    string ItemName,
    uint WorldId,
    decimal MinProfitGil,        // absolute floor
    float MinProfitPercent,      // percentage floor (whichever is higher wins)
    float MinSaleVelocityPerDay,
    float MaxCompetitionScore,
    bool AlertEnabled,
    bool AutoCraftEnabled        // requires Artisan IPC
);

// Domain/Models/AlertEvent.cs
public record AlertEvent(
    Guid Id,
    uint ItemId,
    string ItemName,
    AlertSeverity Severity,
    decimal ProjectedProfit,
    float SaleVelocity,
    float CompetitionScore,
    int SuggestedCraftQuantity,
    DateTimeOffset FiredAt,
    bool Acknowledged
);

// Domain/Models/CraftProfitResult.cs
public record CraftProfitResult(
    uint ItemId,
    decimal MarketPrice,
    decimal CraftCost,           // fully resolved ingredient cost
    decimal ProfitPerUnit,
    float ProfitMarginPercent,
    float SaleVelocityPerDay,
    float CompetitionScore,
    MarketTrend Trend,
    IReadOnlyList<IngredientCost> Ingredients
);
```

---

### 2. Data Layer

#### HTTP Clients

```csharp
// Data/Http/UniversalisClient.cs
public class UniversalisClient
{
    // GET /api/v2/{worldOrDc}/{itemIds}
    Task<MarketResponse?> GetListingsAsync(uint worldId, IEnumerable<uint> itemIds);

    // GET /api/v2/history/{worldOrDc}/{itemId}
    Task<SaleHistoryResponse?> GetSaleHistoryAsync(uint worldId, uint itemId, int entries = 50);

    // Batch: respects Universalis 100-item cap per request
    Task<IReadOnlyDictionary<uint, MarketResponse>> GetBatchAsync(uint worldId, IEnumerable<uint> itemIds);
}

// Data/Http/XivApiClient.cs
public class XivApiClient
{
    Task<RecipeResponse?> GetRecipeAsync(uint recipeId);
    Task<ItemResponse?> GetItemAsync(uint itemId);
    Task<IReadOnlyList<RecipeResponse>> GetRecipesForItemAsync(uint itemId);
}
```

#### Cache

```csharp
// Data/Cache/PriceCache.cs
public class PriceCache
{
    private readonly Dictionary<(uint itemId, uint worldId), CacheEntry<PriceSnapshot>> _store;
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    bool TryGet(uint itemId, uint worldId, out PriceSnapshot snapshot);
    void Set(uint itemId, uint worldId, PriceSnapshot snapshot);
    void Invalidate(uint itemId, uint worldId);
}
```

#### Repositories

All repositories follow the same pattern — interface + EF Core implementation. No raw SQL.

```csharp
public interface IWatchlistRepository
{
    Task<IReadOnlyList<WatchlistEntry>> GetAllAsync();
    Task<WatchlistEntry?> GetByItemIdAsync(uint itemId, uint worldId);
    Task UpsertAsync(WatchlistEntry entry);
    Task RemoveAsync(uint itemId, uint worldId);
}
```

---

### 3. Intelligence Engine

#### ProfitCalculator

Resolves a full ingredient tree recursively, costing each node against either market price or NPC vendor price (whichever is lower, if ItemVendorLocation IPC is available).

```csharp
public class ProfitCalculator
{
    // Full recursive resolution — returns null if prices unavailable
    Task<CraftProfitResult?> CalculateAsync(uint itemId, uint worldId, int quantity = 1);

    // Shallow — only one level of ingredients (faster, less accurate)
    Task<CraftProfitResult?> CalculateShallowAsync(uint itemId, uint worldId);

    private async Task<decimal> ResolveIngredientCostAsync(uint itemId, uint worldId, int quantity);
}
```

#### AlertEvaluator

Stateless — takes a watchlist entry and a profit result, returns an alert or null.

```csharp
public class AlertEvaluator
{
    AlertEvent? Evaluate(WatchlistEntry entry, CraftProfitResult profit);
    // Returns null if thresholds not met
    // Returns AlertEvent if any threshold is crossed
}
```

#### WatchlistProcessor

The main background loop. Coordinates polling → cache → calculation → evaluation → alert dispatch.

```csharp
public class WatchlistProcessor
{
    // Called by BackgroundPoller on tick
    Task ProcessAsync(CancellationToken ct);

    private async Task ProcessEntryAsync(WatchlistEntry entry);
    private void DispatchAlert(AlertEvent alert);

    public event EventHandler<AlertEvent>? AlertFired;
}
```

#### BackgroundPoller

Wraps a `PeriodicTimer` with Dalamud-aware cancellation. Respects player state — does not poll during loading screens or duty content unless configured to.

```csharp
public class BackgroundPoller : IDisposable
{
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource _cts;

    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    public bool PauseInDuty { get; set; } = true;

    Task StartAsync();
    void Stop();
}
```

---

### 4. IPC Bridge

The bridge is the only layer that touches Dalamud IPC APIs. Everything above it operates on domain types.

#### Capability Detection

```csharp
// Ipc/IpcBridge.cs
public class IpcBridge
{
    public IpcCapability DetectedCapabilities { get; private set; }

    public ArtisanCapability? Artisan { get; private set; }
    public AutoRetainerCapability? AutoRetainer { get; private set; }
    public GatherBuddyCapability? GatherBuddy { get; private set; }
    public AllaganToolsCapability? AllaganTools { get; private set; }
    public ItemVendorCapability? ItemVendor { get; private set; }
    public NotificationMasterCapability? NotificationMaster { get; private set; }

    // Called on plugin load and re-evaluated on Framework.Update periodically
    public void RefreshCapabilities();

    public bool Has(IpcCapability capability) =>
        DetectedCapabilities.HasFlag(capability);
}

[Flags]
public enum IpcCapability : uint
{
    None                 = 0,
    Artisan              = 1 << 0,
    AutoRetainer         = 1 << 1,
    GatherBuddy          = 1 << 2,
    AllaganTools         = 1 << 3,
    ItemVendorLocation   = 1 << 4,
    NotificationMaster   = 1 << 5,
    VNavMesh             = 1 << 6,   // required for GBR full auto
}
```

#### Capability Classes

Each capability class wraps Dalamud IPC calls behind a clean interface. Failures are caught and logged — never propagated up.

```csharp
// Ipc/Capabilities/ArtisanCapability.cs
public class ArtisanCapability
{
    // Returns false if Artisan is busy or unavailable
    bool IsAvailable();

    // Enqueues a list of itemId/quantity pairs for crafting
    // Returns false if Artisan rejected the request
    bool TryEnqueueCraftList(IEnumerable<(uint itemId, int quantity)> items);

    // True if Artisan is currently mid-craft
    bool IsCrafting();
}

// Ipc/Capabilities/AutoRetainerCapability.cs
public class AutoRetainerCapability
{
    // Suppress AutoRetainer from firing during a QM-managed sequence
    void Suppress(bool suppressed);

    // Read all current offline character/retainer data
    IReadOnlyList<RetainerState> GetRetainerStates();

    // Read all submarine states
    IReadOnlyList<SubmarineState> GetSubmarineStates();
}

// Ipc/Capabilities/GatherBuddyCapability.cs
public class GatherBuddyCapability
{
    // Queue a gather request for an item
    // vnavmesh required for full auto; falls back to map waypoint if absent
    bool TryQueueGather(uint itemId, int quantity);
}

// Ipc/Capabilities/NotificationMasterCapability.cs
public class NotificationMasterCapability
{
    void SendToast(string title, string body, AlertSeverity severity);
    void SendChatMessage(string message);
}
```

---

### 5. UI Layer

#### AnchorBar

48px wide, vertically centered, rounded on the outside edge only. Renders as a transparent ImGui window with a custom-drawn background.

```
Screen edge
│
│ ┌─╮  ← rounding only on outside
│ │ │  ← icon: Watchlist
│ │ │
│ │ │  ← icon: Alerts  (pip indicator if unread)
│ │ │
│ │ │  ← icon: Market
│ │ │
│ │ │  ← icon: Submarines
│ │ │
│ │ │  ← icon: Retainers
│ │ │
│ │ │  ── spacer ──
│ │ │
│ │ │  ← icon: Settings  (always bottom)
│ └─╯
│
```

Clicking an icon toggles the drawer. Clicking the active icon collapses it. A gold pip renders on the active side edge. Unacknowledged alerts add a red badge count to the Alerts icon.

#### DrawerManager

Owns the currently open panel, manages the drawer window lifecycle.

```csharp
public class DrawerManager
{
    private string? _activeId;
    private readonly IReadOnlyList<ToolbarEntry> _entries;

    public void Toggle(string drawerId);
    public void Close();
    public void Draw();           // called each Framework.Update frame

    // Drawer window: flush against bar, 380px wide, 70% screen height
    private void DrawPanel(ToolbarEntry entry);
}
```

#### Panels

Each panel is a self-contained class implementing `IDrawerPanel`. They receive services via constructor injection and render purely via ImGui calls.

```csharp
public interface IDrawerPanel
{
    string Id { get; }
    string Title { get; }
    string Icon { get; }        // FontAwesome char
    string Tooltip { get; }
    bool HasUnreadBadge { get; }
    void Draw();
}
```

| Panel | Key Responsibilities |
|---|---|
| `WatchlistPanel` | Add/remove items, edit thresholds inline, show current profit state per item |
| `AlertPanel` | Feed of fired alerts newest-first; action buttons: Craft, Gather, Dismiss, Pin |
| `MarketPanel` | Selected item detail: ImPlot price chart, ingredient cost tree, competition score |
| `SubmarinePanel` | Ranked sector list by expected gil/hour; voyage timer if AutoRetainer present |
| `RetainerPanel` | Per-retainer venture status; reassignment suggestions with profit delta |
| `SettingsPanel` | Anchor side toggle, poll interval, IPC status indicators, data cache controls |

#### ConfirmActionModal

All IPC-triggered actions go through this. It is modal — it blocks the drawer until the user responds.

```csharp
public class ConfirmActionModal
{
    public bool IsOpen { get; private set; }

    // Call this before any IPC action
    public void Request(
        string title,
        string description,
        Action onConfirm,
        Action? onAdjust = null,
        Action? onCancel = null
    );

    public void Draw();   // renders the popup if open
}
```

---

### 6. Plugin Entry & Infrastructure

```csharp
// Plugin.cs
[Plugin(Name = "Quartermaster")]
public class Plugin : IDalamudPlugin
{
    public Plugin(IDalamudPluginInterface pi, IFramework framework, ...)
    {
        Services.Initialize(pi, ...);

        // Initialize layers bottom-up
        _db = new QuartermasterDb(...);
        _ipcBridge = new IpcBridge(...);
        _engine = new WatchlistProcessor(...);
        _ui = new AnchorBar(...);

        framework.Update += OnUpdate;
        pi.UiBuilder.Draw += OnDraw;
    }

    private void OnUpdate(IFramework framework)
    {
        _ipcBridge.RefreshCapabilities();   // cheap check, no alloc
    }

    private void OnDraw()
    {
        _ui.Draw();
        _drawerManager.Draw();
        _confirmModal.Draw();
    }

    public void Dispose() { ... }
}
```

---

## Database Schema

```sql
-- Items (cached metadata from XIVAPI)
CREATE TABLE Items (
    ItemId       INTEGER PRIMARY KEY,
    Name         TEXT    NOT NULL,
    IconId       INTEGER NOT NULL,
    CanBeCrafted INTEGER NOT NULL DEFAULT 0,
    VendorPrice  INTEGER,               -- null if not vendor-purchasable
    UpdatedAt    TEXT    NOT NULL
);

-- Watchlist
CREATE TABLE Watchlist (
    ItemId              INTEGER NOT NULL,
    WorldId             INTEGER NOT NULL,
    MinProfitGil        REAL    NOT NULL DEFAULT 0,
    MinProfitPercent    REAL    NOT NULL DEFAULT 0,
    MinVelocityPerDay   REAL    NOT NULL DEFAULT 0,
    MaxCompetitionScore REAL    NOT NULL DEFAULT 9999,
    AlertEnabled        INTEGER NOT NULL DEFAULT 1,
    AutoCraftEnabled    INTEGER NOT NULL DEFAULT 0,
    AddedAt             TEXT    NOT NULL,
    PRIMARY KEY (ItemId, WorldId)
);

-- Price history (rolling window — prune entries older than 30 days)
CREATE TABLE PriceHistory (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    ItemId      INTEGER NOT NULL,
    WorldId     INTEGER NOT NULL,
    MinPrice    INTEGER NOT NULL,
    AvgPrice    INTEGER NOT NULL,
    Quantity    INTEGER NOT NULL,
    SnappedAt   TEXT    NOT NULL
);
CREATE INDEX idx_price_item_world ON PriceHistory(ItemId, WorldId);
CREATE INDEX idx_price_snapped    ON PriceHistory(SnappedAt);

-- Recipe graphs (cached from XIVAPI)
CREATE TABLE Recipes (
    RecipeId    INTEGER PRIMARY KEY,
    ItemId      INTEGER NOT NULL,
    ResultQty   INTEGER NOT NULL,
    Ingredients TEXT    NOT NULL,   -- JSON: [{itemId, qty, isNpc}]
    CachedAt    TEXT    NOT NULL
);

-- Alert history (audit log, kept indefinitely unless user clears)
CREATE TABLE AlertHistory (
    Id               TEXT    PRIMARY KEY,   -- GUID
    ItemId           INTEGER NOT NULL,
    Severity         TEXT    NOT NULL,
    ProjectedProfit  REAL    NOT NULL,
    SaleVelocity     REAL    NOT NULL,
    CompetitionScore REAL    NOT NULL,
    SuggestedQty     INTEGER NOT NULL,
    FiredAt          TEXT    NOT NULL,
    Acknowledged     INTEGER NOT NULL DEFAULT 0,
    ActedOn          INTEGER NOT NULL DEFAULT 0
);

-- Submarine sectors (seeded from community tables, user can override)
CREATE TABLE SubmarineSectors (
    SectorId         INTEGER PRIMARY KEY,
    SectorName       TEXT    NOT NULL,
    AverageGilPerHour REAL   NOT NULL,
    VoyageDurationMin INTEGER NOT NULL,
    DropTableJson    TEXT    NOT NULL,   -- JSON: [{itemId, dropRate}]
    UpdatedAt        TEXT    NOT NULL
);

-- Venture history (populated via AutoRetainer IPC or manual entry)
CREATE TABLE VentureHistory (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    RetainerName  TEXT    NOT NULL,
    VentureTypeId INTEGER NOT NULL,
    ItemId        INTEGER,
    Quantity      INTEGER,
    CompletedAt   TEXT    NOT NULL,
    WorldId       INTEGER NOT NULL
);
```

---

## IPC Integration Map

```
QM Action                     → Target Plugin         → IPC Method / Approach
──────────────────────────────────────────────────────────────────────────────
Check ingredient availability → AllaganTools          → GetItemCount(itemId)
Resolve NPC vendor price      → ItemVendorLocation    → GetVendorPrice(itemId)
Send toast notification       → NotificationMaster    → SendToast(...)
Enqueue craft list            → Artisan               → AddItemsToQueue([...])
Check craft in progress       → Artisan               → IsCrafting()
Suppress AR during sequence   → AutoRetainer          → SetSuppressed(true)
Read retainer venture states  → AutoRetainer          → GetRetainerData()
Read submarine states         → AutoRetainer          → GetSubmarineData()
Queue material gather         → GatherBuddyReborn     → InvokeGather(itemId, qty)
Check vnavmesh present        → PluginDetector        → DalamudReflector check
Fallback (no IPC available)   → Something Need Doing  → /snd command emission
```

### Graceful Degrade Table

| Feature | Full IPC | Partial (no Artisan) | No IPC at all |
|---|---|---|---|
| Profit alerts | ✅ | ✅ | ✅ |
| Craft queue | Auto via Artisan | Manual prompt | Manual prompt |
| Ingredient check | Via AllaganTools | Via DB + game memory | Estimate only |
| NPC pricing | Via ItemVendor | Skipped in cost calc | Skipped |
| Toasts | Via NotifMaster | Dalamud native toast | Chat message |
| Gathering | Via GatherBuddy | Map flag only | Map flag only |
| Retainer data | Via AutoRetainer | Manual entry | Manual entry |
| Submarine data | Via AutoRetainer | Manual entry | Manual entry |

---

## UI Structure

```
AnchorBar (48px, screen edge, vertically centered)
│
├── [W] Watchlist drawer
│     ├── Search/add item bar
│     ├── Item list rows (icon, name, current profit, trend arrow, active badge)
│     └── Expand row → inline threshold editor
│
├── [A] Alerts drawer  (badge count when unread)
│     ├── Filter bar: All / Opportunity / Warning / Urgent
│     ├── Alert cards (newest first)
│     │     ├── Item icon + name + severity color
│     │     ├── Profit projection + velocity + competition
│     │     └── Action buttons: [Craft] [Gather] [List] [Dismiss]
│     └── "Clear all" button
│
├── [M] Market drawer
│     ├── Item selector (defaults to last-viewed)
│     ├── Price chart (ImPlot — 7d history, zoom-able)
│     ├── Current listings table (qty, price, retainer, world)
│     ├── Ingredient cost tree (recursive, collapsible nodes)
│     └── Profit summary chip
│
├── [S] Submarines drawer
│     ├── Active voyages (timer, route, expected return)
│     ├── Sector ranking table (name, avg gil/hr, voyage time, next recommended)
│     └── [Send Fleet] button (requires AutoRetainer IPC + user confirm)
│
├── [R] Retainers drawer
│     ├── Per-retainer cards (name, current venture, time remaining)
│     ├── Reassignment suggestions (current vs suggested, profit delta)
│     └── [Reassign] button (requires AutoRetainer IPC + user confirm)
│
├── ── spacer ──
│
└── [⚙] Settings drawer
      ├── UI: anchor side toggle (Left / Right), bar opacity
      ├── Engine: poll interval, world selection, currency display
      ├── IPC Status panel: detected plugins with green/grey/red indicators
      ├── Data: cache age display, [Clear cache] [Re-seed submarine data]
      └── About: version, links, credits
```

---

## Configuration Model

```csharp
public class PluginConfig : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    // UI
    public AnchorSide AnchorSide { get; set; } = AnchorSide.Left;
    public float BarOpacity { get; set; } = 0.92f;
    public string? LastOpenDrawer { get; set; }

    // Engine
    public uint HomeWorldId { get; set; }
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(5);
    public bool PausePollInDuty { get; set; } = true;
    public bool PausePollInCombat { get; set; } = false;

    // Alert preferences
    public bool AlertsViaNotificationMaster { get; set; } = true;
    public bool AlertsViaToast { get; set; } = true;
    public bool AlertsViaChat { get; set; } = false;
    public AlertSeverity MinAlertSeverity { get; set; } = AlertSeverity.Info;

    // IPC behaviour
    public bool RequireConfirmBeforeCraft { get; set; } = true;  // always true, not user-editable
    public bool AutoSuppressRetainerDuringCraft { get; set; } = true;

    // Submarine & retainer
    public float SubmarineAlertThresholdGilPerHour { get; set; } = 50_000;
    public bool SubmarineAutoSuggest { get; set; } = true;
}
```

---

## Background Worker Model

```
Framework.Update (every frame)
    └── IpcBridge.RefreshCapabilities()   [cheap: no alloc, flag compare only]

PeriodicTimer (configurable, default 5 min)
    └── WatchlistProcessor.ProcessAsync()
          ├── For each WatchlistEntry:
          │     ├── Check PriceCache (TTL)
          │     ├── If stale → UniversalisClient.GetBatchAsync()
          │     ├── ProfitCalculator.CalculateAsync()
          │     ├── AlertEvaluator.Evaluate()
          │     └── If alert → AlertRepository.SaveAsync() + DispatchAlert()
          │
          ├── SubmarineAdvisor.RefreshAsync()   [if AutoRetainer present]
          └── RetainerAdvisor.RefreshAsync()    [if AutoRetainer present]

Alert dispatch path:
    AlertEvent
        ├── AlertRepository.SaveAsync()          (persist to SQLite)
        ├── NotificationMasterCapability.SendToast()  (if available)
        ├── Dalamud toast fallback               (if not)
        └── AlertPanel.OnAlertReceived()         (updates badge count)
```

---

## Phased Delivery Plan

### Phase 1 — Foundation *(~2 weeks)*
- `Plugin.cs` entry point, `Services.cs`, `PluginConfig`
- SQLite schema + EF Core setup + basic migrations
- `UniversalisClient` + `XivApiClient` with retry/timeout
- `PriceCache` + `RecipeCache`
- `SettingsPanel` (anchor side, world select, IPC status stubs)
- `AnchorBar` + `DrawerManager` (UI shell, no content yet)

**Milestone:** Plugin loads, bar renders on screen edge, settings save/load correctly.

---

### Phase 2 — Intelligence Core *(~2 weeks)*
- `WatchlistRepository` + `PriceRepository` + `RecipeRepository`
- `ProfitCalculator` (recursive ingredient costing)
- `VelocityTracker` (sliding window on `PriceHistory`)
- `AlertEvaluator` (threshold comparison, severity assignment)
- `WatchlistProcessor` + `BackgroundPoller`
- `WatchlistPanel` (add/edit/remove items, threshold editor)
- `AlertPanel` (feed, dismiss, basic action buttons)

**Milestone:** Add an item to the watchlist, see live profit calculations, receive alerts in-game when thresholds are crossed.

---

### Phase 3 — Market & Advisor UI *(~2 weeks)*
- `MarketPanel` (ImPlot price chart, listing table, ingredient tree)
- `SubmarineAdvisor` + `SubmarinePanel` (static data, no IPC yet)
- `RetainerAdvisor` + `RetainerPanel` (static data, no IPC yet)
- `CompetitionEstimator` (listing count heuristic)
- `ProfitBadge`, `TrendArrow`, `AlertCard`, `IngredientTree` components
- Polish: icon loading, colour theming, panel layouts

**Milestone:** Full read-only dashboard — all five drawers functional with real data, no IPC yet.

---

### Phase 4 — IPC Orchestration *(~3 weeks)*
- `IpcBridge` + all capability classes
- `ConfirmActionModal`
- `AllaganToolsCapability` — ingredient resolution upgrade
- `ItemVendorCapability` — NPC price sourcing in cost calc
- `NotificationMasterCapability` — route all alerts through it
- `ArtisanCapability` — "Craft" button in `AlertPanel` live
- `AutoRetainerCapability` — submarine/retainer panels go live
- `GatherBuddyCapability` — "Gather" button in `AlertPanel` live
- `IpcStatusIndicator` component in `SettingsPanel`
- Graceful degrade testing for all partial-IPC scenarios

**Milestone:** Full orchestration loop — alert fires, user clicks Craft, Artisan queues items. All IPC paths degrade cleanly if plugins are absent.

---

### Phase 5 — Telemetry Backend *(separate project, post-v1)*
- ASP.NET Core minimal API on a VPS
- Endpoint: `POST /telemetry/craft-signal` (itemId, worldId, timestamp — no PII)
- Aggregation: competition pressure score per (itemId, worldId) per hour
- QM polls this endpoint, incorporates score into `AlertEvaluator`
- Opt-in setting, clearly disclosed in UI
- Separate plugin feature flag, not part of base install

---

## Key Technical Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Local storage | SQLite via EF Core | Relational queries needed for recipe graphs; EF handles migrations cleanly |
| HTTP client | `IHttpClientFactory` + `HttpClient` | Pooling, lifetime management, easy to mock in tests |
| Price polling | HTTP polling (Phase 1), WebSocket upgrade (Phase 4) | Polling is simpler to ship; WS adds value only for hot watchlist items |
| Recipe cost resolution | Recursive with NPC floor | Accurate ingredient costing; NPC check prevents overpaying via market |
| IPC failure handling | Always degrade, never throw | IPC partners update independently; hard failures would break QM on patch day |
| UI rendering | ImGui only | Stays consistent with Dalamud ecosystem; no WebView overhead |
| Alert dispatch | Event-based (`AlertFired` event) | Decouples engine from UI and notification delivery |
| Telemetry | Phase 5, separate project | Avoids scope creep; different trust model and infrastructure needs |
| Automation safety | `ConfirmActionModal` always required | Non-negotiable — QM never fires an IPC action without user confirmation |

---

## Dependency Graph

```
Plugin.cs
    ├── IpcBridge
    │     ├── ArtisanCapability
    │     ├── AutoRetainerCapability
    │     ├── GatherBuddyCapability
    │     ├── AllaganToolsCapability
    │     ├── ItemVendorCapability
    │     └── NotificationMasterCapability
    │
    ├── WatchlistProcessor
    │     ├── ProfitCalculator
    │     │     ├── UniversalisClient  ──► Universalis API
    │     │     ├── XivApiClient       ──► XIVAPI
    │     │     ├── PriceCache
    │     │     ├── RecipeCache
    │     │     └── ItemVendorCapability (optional)
    │     ├── VelocityTracker
    │     │     └── PriceRepository
    │     ├── AlertEvaluator
    │     │     └── WatchlistRepository
    │     └── BackgroundPoller
    │
    ├── SubmarineAdvisor
    │     ├── SubmarineRepository
    │     └── AutoRetainerCapability (optional)
    │
    ├── RetainerAdvisor
    │     ├── VentureRepository
    │     └── AutoRetainerCapability (optional)
    │
    └── UI
          ├── AnchorBar
          ├── DrawerManager
          │     ├── WatchlistPanel     ──► WatchlistRepository, ProfitCalculator
          │     ├── AlertPanel         ──► AlertRepository, IpcBridge
          │     ├── MarketPanel        ──► PriceRepository, RecipeRepository
          │     ├── SubmarinePanel     ──► SubmarineAdvisor
          │     ├── RetainerPanel      ──► RetainerAdvisor
          │     └── SettingsPanel      ──► PluginConfig, IpcBridge
          └── ConfirmActionModal
```

---

*Document maintained alongside development. Update version and dates when architecture decisions change.*

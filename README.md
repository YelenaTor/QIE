# QIE — Quartermaster Intelligence Engine

> *Standing on the shoulders of giants.*

## Acknowledgements

QIE would not exist without the incredible work of the Dalamud community. Special thanks to:

- **[Una](https://github.com/una-xiv)** — Creator of [Umbra](https://github.com/una-xiv/umbra) and [Una.Drawing](https://github.com/una-xiv/drawing), the retained-mode UI engine that powers QIE's interface. Umbra's approach to plugin UI design set the bar for what's possible in Dalamud.
- **[NickReinlein](https://github.com/NickReinlein)** — Creator of [GilGoblin](https://github.com/NickReinlein/GilGoblin), the plugin that inspired QIE. GilGoblin proved that intelligent, data-driven crafting tools have a real place in the FFXIV ecosystem.
- **[NightmareXIV](https://github.com/NightmareXIV)** — Creator of [ECommons](https://github.com/NightmareXIV/ECommons), the shared library that makes Dalamud plugin development sane.
- **[Ottermandias](https://github.com/Ottermandias)** — Creator of [OtterGui](https://github.com/Ottermandias/OtterGui), used throughout for ImGui utilities.

---

![GitHub release](https://img.shields.io/github/v/release/YelenaTor/QIE?style=flat-square)
![License](https://img.shields.io/github/license/YelenaTor/QIE?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/YelenaTor/QIE?style=flat-square)

---

## What is QIE?

**Quartermaster Intelligence Engine** is a Dalamud plugin for FFXIV that consolidates market, crafting, retainer, and submarine profit data into a single always-available dashboard anchored to the edge of your game screen.

QIE watches the markets so you don't have to.

### The Vision

Most crafters and market-board players alt-tab between spreadsheets, Universalis, and multiple plugins to answer one question: *"What should I be making right now?"*

QIE answers that question automatically by:

- **Watching your watchlist** — Continuously polling Universalis for price changes on items you care about
- **Calculating real profit** — Recursive recipe costing that accounts for sub-components, NPC materials, and current market prices
- **Alerting you to opportunities** — Configurable thresholds that fire when profit margins, velocity, or competition levels hit your targets
- **Orchestrating your tools** — One-click actions that talk to Artisan, AutoRetainer, and GatherBuddy via IPC, always with your explicit approval
- **Advising on submarines & retainers** — Ranks routes and ventures by expected gil/hour based on your data

### Architecture

QIE is built as a layered plugin:

| Layer | Purpose |
|-------|---------|
| **Configuration** | User preferences, watchlist thresholds, alert rules |
| **Domain** | Pure data models and enums — no Dalamud dependencies |
| **Data** | SQLite via EF Core, Universalis/XIVAPI clients, TTL caches |
| **Engine** | Profit calculator, velocity tracker, alert evaluator, background poller |
| **IPC** | Capability wrappers for Artisan, AutoRetainer, GatherBuddy, AllaganTools, and more |
| **UI** | [Una.Drawing](https://github.com/una-xiv/drawing)-powered interface with XML/CSS templates |

### UI

QIE's interface uses a dock-and-drawer pattern inspired by Umbra:

- **Anchor Bar** — A 48px icon strip pinned to the screen edge (left or right)
- **Drawer Panels** — 380px overlay panels that slide out when you click an icon:
  - 📋 **Watchlist** — Your tracked items with live prices and profit projections
  - 🔔 **Alerts** — Actionable opportunities sorted by priority
  - 📊 **Market** — Deep-dive into any item: recipe tree, competition, velocity
  - 🚢 **Submarines** — Voyage advisor with sector rankings
  - 👤 **Retainers** — Venture status and reassignment suggestions
  - ⚙️ **Settings** — Preferences, IPC status, cache management

### Current Status

> 🚧 **Pre-alpha** — The skeleton is built, the UI framework is wired, but core functionality is not yet implemented. See the [releases](https://github.com/YelenaTor/QIE/releases) for the latest state.

## Installation

QIE is not yet available via the standard Dalamud plugin repository. For development/testing:

1. Add the custom repo URL to Dalamud's plugin installer:
   ```
   https://yelenator.github.io/QIE/repo.json
   ```
2. Search for "Quartermaster" in the plugin list and install.

## Building from Source

```bash
# Clone with submodules
git clone --recursive https://github.com/YelenaTor/QIE.git

# Ensure Dalamud dev binaries are installed
# (default: %APPDATA%\XIVLauncher\addon\Hooks\dev\)

# Build
dotnet restore
dotnet build --configuration Release
```

## License

QIE is licensed under the [GNU Affero General Public License v3.0](LICENSE).

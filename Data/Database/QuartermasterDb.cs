using Microsoft.EntityFrameworkCore;
using Quartermaster.Data.Database.Entities;

namespace Quartermaster.Data.Database;

/// <summary>
/// EF Core DbContext for the Quartermaster SQLite database.
/// </summary>
public class QuartermasterDb : DbContext, IDisposable
{
    public DbSet<ItemEntity> Items { get; set; } = null!;
    public DbSet<WatchlistEntity> Watchlist { get; set; } = null!;
    public DbSet<PriceHistoryEntity> PriceHistory { get; set; } = null!;
    public DbSet<AlertHistoryEntity> AlertHistory { get; set; } = null!;
    public DbSet<RecipeEntity> Recipes { get; set; } = null!;
    public DbSet<SubmarineEntity> SubmarineSectors { get; set; } = null!;
    public DbSet<VentureHistoryEntity> VentureHistory { get; set; } = null!;

    private readonly string _dbPath;

    public QuartermasterDb()
    {
        var pluginDir = Plugin.PluginInterface.GetPluginConfigDirectory();
        _dbPath = Path.Combine(pluginDir, "quartermaster.db");
    }

    /// <summary>
    /// Create the database file and tables if they don't exist.
    /// Call once on plugin load.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            await Database.EnsureCreatedAsync();
            Plugin.Log.Information($"[QM] Database initialized at: {_dbPath}");
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex, "[QM] Failed to initialize database.");
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Items
        modelBuilder.Entity<ItemEntity>(e =>
        {
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).ValueGeneratedNever();
        });

        // Watchlist — composite PK
        modelBuilder.Entity<WatchlistEntity>(e =>
        {
            e.HasKey(x => new { x.ItemId, x.WorldId });
        });

        // PriceHistory — autoincrement
        modelBuilder.Entity<PriceHistoryEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasIndex(x => new { x.ItemId, x.WorldId });
            e.HasIndex(x => x.SnappedAt);
        });

        // AlertHistory — GUID PK
        modelBuilder.Entity<AlertHistoryEntity>(e =>
        {
            e.HasKey(x => x.Id);
        });

        // Recipes
        modelBuilder.Entity<RecipeEntity>(e =>
        {
            e.HasKey(x => x.RecipeId);
            e.Property(x => x.RecipeId).ValueGeneratedNever();
        });

        // SubmarineSectors
        modelBuilder.Entity<SubmarineEntity>(e =>
        {
            e.HasKey(x => x.SectorId);
            e.Property(x => x.SectorId).ValueGeneratedNever();
        });

        // VentureHistory — autoincrement
        modelBuilder.Entity<VentureHistoryEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
    }
}

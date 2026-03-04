namespace Quartermaster.Domain.Models;

public record WatchlistEntry(
    uint ItemId,
    string ItemName,
    uint WorldId,
    decimal MinProfitGil,
    float MinProfitPercent,
    float MinSaleVelocityPerDay,
    float MaxCompetitionScore,
    bool AlertEnabled,
    bool AutoCraftEnabled
);

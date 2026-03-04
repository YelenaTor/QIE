using Quartermaster.Domain.Enums;

namespace Quartermaster.Domain.Models;

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

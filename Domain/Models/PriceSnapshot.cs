namespace Quartermaster.Domain.Models;

public record PriceSnapshot(
    uint ItemId,
    uint WorldId,
    int MinPrice,
    int AvgPrice,
    int Quantity,
    DateTimeOffset SnappedAt
);

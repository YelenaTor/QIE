namespace Quartermaster.Domain.Models;

public record VentureResult(
    long Id,
    string RetainerName,
    int VentureTypeId,
    uint? ItemId,
    int? Quantity,
    DateTimeOffset CompletedAt,
    uint WorldId
);

namespace Quartermaster.Domain.Models;

public record CompetitionScore(
    uint ItemId,
    uint WorldId,
    int ActiveListingCount,
    float Score,
    DateTimeOffset EvaluatedAt
);

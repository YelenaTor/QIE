namespace Quartermaster.Domain.Models;

public record SubmarineRoute(
    int SectorId,
    string SectorName,
    float AverageGilPerHour,
    int VoyageDurationMinutes,
    IReadOnlyList<SubmarineDrop> DropTable
);

public record SubmarineDrop(
    uint ItemId,
    float DropRate
);

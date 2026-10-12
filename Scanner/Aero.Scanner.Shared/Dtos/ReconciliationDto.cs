using Aero.Scanner.Domain.Enums;

namespace Aero.Scanner.Shared.Dtos;

public sealed record ReconciliationDto(
    int Id,
    ReconciliationStatus Status,
    int TotalChecked,
    int TotalLoaded,
    int TotalOffloaded,
    int TotalTransferred,
    IReadOnlyList<string> UnmatchedBags,
    IReadOnlyList<string> OffloadedBags,
    IReadOnlyList<string> RushBags,
    DateTime? ClosedAt,
    string? ClosedBy);

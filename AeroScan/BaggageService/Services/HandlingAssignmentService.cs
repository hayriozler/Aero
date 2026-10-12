using BaggageService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Services;

public sealed class HandlingAssignmentService(AeroScanDataContext db)
{
    public async Task<string?> ResolveAsync(
        string flightNumber,
        string airlineCode,
        string? airlineCode2,
        string? airlineCode3,
        DateTime? scheduledDeparture,
        CancellationToken ct = default)
    {
        if (!scheduledDeparture.HasValue) return null;

        var sch = scheduledDeparture.Value;

        // Collect all non-empty codes the airline is known by
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(airlineCode))  codes.Add(airlineCode);
        if (!string.IsNullOrWhiteSpace(airlineCode2)) codes.Add(airlineCode2);
        if (!string.IsNullOrWhiteSpace(airlineCode3)) codes.Add(airlineCode3);

        if (codes.Count == 0) return null;

        var candidates = await db.HandlingContractSet
            .Include(c => c.FlightNumbers)
            .Where(c => c.IsActive
                     && codes.Contains(c.AirlineCode)
                     && c.ValidFrom <= sch
                     && c.ValidTo   >= sch)
            .ToListAsync(ct);

        if (candidates.Count == 0) return null;

        // Specific contracts (with an explicit flight number list) take priority
        var specific = candidates
            .Where(c => c.IsSpecific &&
                        c.FlightNumbers.Any(fn =>
                            fn.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase)))
            .MaxBy(c => c.CreatedAt);

        if (specific is not null) return specific.HandlingCompanyCode;

        // Fall back to general (airline-wide) contracts
        var general = candidates
            .Where(c => !c.IsSpecific)
            .MaxBy(c => c.CreatedAt);

        return general?.HandlingCompanyCode;
    }
}

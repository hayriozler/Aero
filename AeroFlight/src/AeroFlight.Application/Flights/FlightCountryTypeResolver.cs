using AeroFlight.Application.Interfaces;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights;

public interface IFlightCountryTypeResolver
{
    Task<string> ResolveAsync(IEnumerable<string?> airportCodes, CancellationToken ct);
}
public sealed class FlightCountryTypeResolver(IAeroFlightDbContext db)
    : IFlightCountryTypeResolver
{
    public async Task<string> ResolveAsync(IEnumerable<string?> airportCodes, CancellationToken ct)
    {
        var codes = airportCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (codes.Count == 0)
            throw new ArgumentException("No valid airport codes provided.", nameof(airportCodes));

        var foundAirportIds = await db.Query<Airport>()
            .Where(a => codes.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(ct);

        var missing = codes.Except(foundAirportIds, StringComparer.OrdinalIgnoreCase).ToList();
        if (missing.Count > 0)
            throw new KeyNotFoundException($"Airport code(s) not found in Metadata data: {string.Join(", ", missing)}.");

        var countryTypeCodes = await db.Query<Airport>()
            .Where(a => codes.Contains(a.Id))
            .Join(
                db.Query<Country>(),
                airport => airport.CountryCode,
                country => country.Id,
                (airport, country) => country.CountryTypeCode)
            .Distinct()
            .Join(
                db.Query<CountryType>(),
                countryTypeCode => countryTypeCode,
                countryType => countryType.Code,
                (countryTypeCode, countryType) => new { CountryTypeCode = countryTypeCode, countryType.Severity })
            .OrderByDescending(x => x.Severity)
            .Select(x => x.CountryTypeCode)
            .ToListAsync(ct);

        return countryTypeCodes.First();
    }
}
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops;

namespace Aero.Flights.Application.Flights.Commands;

internal sealed record CommonFlightUpdateFields(
    string? CallSign,
    int? CodeshareNumber,
    string FlightTypeCode,
    string? AircraftCode,
    string? AircraftTypeCode,
    string? OperatingAirlineCode,
    string? TerminalCode,
    string? ParkPositionCode,
    string? HandlingAgentCode,
    string? Comment,
    int? PaxTotal,
    int? PaxTransit,
    int? PaxAssistanceRequired,
    int? Pax1,
    int? Pax2,
    int? Pax3,
    int? CrewNumber,
    int? CrewCockpit,
    decimal? BaggageWeight,
    decimal? BaggageTotal,
    decimal? CargoWeight,
    decimal? MailWeight,
    decimal? TotalWeight,
    decimal? EquipmentWeight,
    decimal? CabinLuggage);

internal static class UpdateFlightSteps
{
    public static void ApplyFlightNumber(Flight flight, string normalizedFlightNumber)
    {
        if (!string.Equals(flight.FlightNumber, normalizedFlightNumber, StringComparison.OrdinalIgnoreCase))
            flight.UpdateFlightNumber(normalizedFlightNumber);
    }

    public static void ApplyStatus(Flight flight, string statusCode)
    {
        if (statusCode == FlightStatusCodes.Cancelled)
        {
            if (flight.Status != FlightStatusCodes.Cancelled)
                flight.Cancel(DateTimeOffset.UtcNow);
        }
        else
        {
            flight.FlightUpdateStatus(statusCode);
        }
    }

    public static async Task ApplyCommonFieldsAsync(Flight flight, IFlightCountryTypeResolver countryTypeResolver,
        IEnumerable<string?> routingCodes, CommonFlightUpdateFields f, CancellationToken ct)
    {
        flight.UpdateCallSign(f.CallSign);
        flight.UpdateCodeshareNumber(f.CodeshareNumber);
        flight.UpdateFlightTypeCode(f.FlightTypeCode);
        flight.UpdateCountryType(await countryTypeResolver.ResolveAsync(routingCodes, ct));
        flight.UpdateAircraft(f.AircraftCode, f.AircraftTypeCode, f.OperatingAirlineCode, null);
        flight.UpdateTerminal(f.TerminalCode);
        flight.UpdateParkPosition(f.ParkPositionCode);
        flight.UpdateHandling(f.HandlingAgentCode);
        flight.UpdateComment(f.Comment);
        flight.UpdatePaxCounts(f.PaxTotal, f.PaxTransit, f.PaxAssistanceRequired, f.Pax1, f.Pax2, f.Pax3, f.CrewNumber, f.CrewCockpit);
        flight.UpdateWeightData(f.BaggageWeight, f.BaggageTotal, f.CargoWeight, f.MailWeight, f.TotalWeight, f.EquipmentWeight, f.CabinLuggage);
    }

    // Only re-attempt linking when the registration itself changed — otherwise an operator who
    // deliberately unlinked two flights would have them silently re-linked by the next unrelated
    // edit (e.g. a schedule change).
    public static Task RelinkIfAircraftChangedAsync(string? previousAircraftCode, string? newAircraftCode,
        CancellationToken ct, Func<CancellationToken, Task> relink)
        => string.Equals(previousAircraftCode, newAircraftCode, StringComparison.OrdinalIgnoreCase)
            ? Task.CompletedTask
            : relink(ct);
}

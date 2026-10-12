using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Rules;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Commands;

// Union of every field any creation source (API, RabbitMQ inbound, seasonal schedule) can supply.
public sealed record ArrivalFlightCreationInput(
    string AirlineCode, string FlightNumber, DateTimeOffset Sta, string OriginAirportCode,
    IReadOnlyList<(string AirportCode, int LegNumber)>? ExtraRoutings = null,
    string? FlightTypeCode = null, string? StatusCode = null,
    string? CallSign = null, int? CodeshareNumber = null,
    IReadOnlyList<(string AirlineCode, string FlightNumber)>? Codeshares = null,
    string? AircraftCode = null, string? AircraftTypeCode = null, string? OperatingAirlineCode = null,
    string? TerminalCode = null, string? HandlingAgentCode = null, string? Comment = null,
    int? PaxTotal = null, string? DefaultGateCode = null);

public sealed record DepartureFlightCreationInput(
    string AirlineCode, string FlightNumber, DateTimeOffset Std, string DestinationAirportCode,
    IReadOnlyList<(string AirportCode, int LegNumber)>? ExtraRoutings = null,
    string? FlightTypeCode = null, string? StatusCode = null,
    string? CallSign = null, int? CodeshareNumber = null,
    IReadOnlyList<(string AirlineCode, string FlightNumber)>? Codeshares = null,
    string? AircraftCode = null, string? AircraftTypeCode = null, string? OperatingAirlineCode = null,
    string? TerminalCode = null, string? HandlingAgentCode = null, string? Comment = null,
    int? PaxTotal = null, string? DefaultGateCode = null);

// Single place every flight-creation source funnels through, regardless of where it came from.
public interface IFlightCreationService
{
    Task<(CreateFlightOutcome Outcome, ArrivalFlight? Flight)> CreateArrivalAsync(ArrivalFlightCreationInput input, CancellationToken ct);
    Task<(CreateFlightOutcome Outcome, DepartureFlight? Flight)> CreateDepartureAsync(DepartureFlightCreationInput input, CancellationToken ct);
}

public sealed class FlightCreationService(
    IAeroFlightDbContext db, IRuleEngine ruleEngine, IFlightCountryTypeResolver countryTypeResolver,
    IFlightAutoLinker autoLinker, IResourceDefaultStatusResolver statusResolver)
    : IFlightCreationService
{
    public async Task<(CreateFlightOutcome Outcome, ArrivalFlight? Flight)> CreateArrivalAsync(ArrivalFlightCreationInput input, CancellationToken ct)
    {
        var flightNumber = FlightNumberFormat.Normalize(input.FlightNumber);
        var flightDate = DateOnly.FromDateTime(input.Sta.Date);

        var duplicate = await db.Query<ArrivalFlight>()
            .AnyAsync(f => f.AirlineCode == input.AirlineCode && f.FlightNumber == flightNumber && f.FlightDate == flightDate, ct);
        if (duplicate)
            return (CreateFlightOutcome.DuplicateFlight, null);

        var flight = ArrivalFlight.Create(input.AirlineCode, flightNumber, input.Sta, input.OriginAirportCode);
        foreach (var (code, legNumber) in input.ExtraRoutings ?? [])
            flight.AddRouting(code, legNumber);

        if (input.FlightTypeCode is { } flightTypeCode) flight.UpdateFlightTypeCode(flightTypeCode);
        if (input.StatusCode is { } statusCode) flight.FlightUpdateStatus(statusCode);
        flight.UpdateCallSign(input.CallSign);
        flight.UpdateCodeshareNumber(input.CodeshareNumber);
        foreach (var cs in input.Codeshares ?? []) flight.AddCodeShare(cs.AirlineCode, cs.FlightNumber);
        flight.UpdateCountryType(await countryTypeResolver.ResolveAsync([.. flight.Routings.Select(s => s.Code)], ct));
        flight.UpdateAircraft(input.AircraftCode, input.AircraftTypeCode, input.OperatingAirlineCode, null);
        flight.UpdateTerminal(input.TerminalCode);
        flight.UpdateHandling(input.HandlingAgentCode);
        flight.UpdateComment(input.Comment);
        flight.UpdatePaxCounts(input.PaxTotal, null, null, null, null, null, null, null);

        if (!string.IsNullOrWhiteSpace(input.DefaultGateCode))
            await statusResolver.EnsureDefaultStatusAsync(flight.AddGate(input.DefaultGateCode), ct);

        await ruleEngine.EvaluateAsync(RuleTrigger.ArrivalCreated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightCreated, flight, ct);

        db.Add(flight);
        await db.SaveChangesAsync(ct);

        await autoLinker.TryLinkArrivalAsync(flight, ct);

        return (CreateFlightOutcome.Created, flight);
    }

    public async Task<(CreateFlightOutcome Outcome, DepartureFlight? Flight)> CreateDepartureAsync(DepartureFlightCreationInput input, CancellationToken ct)
    {
        var flightNumber = FlightNumberFormat.Normalize(input.FlightNumber);
        var flightDate = DateOnly.FromDateTime(input.Std.Date);

        var duplicate = await db.Query<DepartureFlight>()
            .AnyAsync(f => f.AirlineCode == input.AirlineCode && f.FlightNumber == flightNumber && f.FlightDate == flightDate, ct);
        if (duplicate)
            return (CreateFlightOutcome.DuplicateFlight, null);

        var flight = DepartureFlight.Create(input.AirlineCode, flightNumber, input.Std, input.DestinationAirportCode);
        foreach (var (code, legNumber) in input.ExtraRoutings ?? [])
            flight.AddRouting(code, legNumber);

        if (input.FlightTypeCode is { } flightTypeCode) flight.UpdateFlightTypeCode(flightTypeCode);
        if (input.StatusCode is { } statusCode) flight.FlightUpdateStatus(statusCode);
        flight.UpdateCallSign(input.CallSign);
        flight.UpdateCodeshareNumber(input.CodeshareNumber);
        foreach (var cs in input.Codeshares ?? []) flight.AddCodeShare(cs.AirlineCode, cs.FlightNumber);
        flight.UpdateCountryType(await countryTypeResolver.ResolveAsync([.. flight.Routings.Select(s => s.Code)], ct));
        flight.UpdateAircraft(input.AircraftCode, input.AircraftTypeCode, input.OperatingAirlineCode, null);
        flight.UpdateTerminal(input.TerminalCode);
        flight.UpdateHandling(input.HandlingAgentCode);
        flight.UpdateComment(input.Comment);
        flight.UpdatePaxCounts(input.PaxTotal, null, null, null, null, null, null, null);

        if (!string.IsNullOrWhiteSpace(input.DefaultGateCode))
            await statusResolver.EnsureDefaultStatusAsync(flight.AddGate(input.DefaultGateCode), ct);

        await ruleEngine.EvaluateAsync(RuleTrigger.DepartureCreated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightCreated, flight, ct);

        db.Add(flight);
        await db.SaveChangesAsync(ct);

        await autoLinker.TryLinkDepartureAsync(flight, ct);

        return (CreateFlightOutcome.Created, flight);
    }
}

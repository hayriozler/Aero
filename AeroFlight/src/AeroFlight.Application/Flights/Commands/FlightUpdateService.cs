using AeroFlight.Application.Common;
using AeroFlight.Application.Flights.InboundProcessors;
using AeroFlight.Application.Flights.Queries;
using AeroFlight.Application.Flights.Resources;
using AeroFlight.Application.Interfaces;
using AeroFlight.Application.Rules;
using AeroFlight.Contracts.Flights;
using AeroFlight.Contracts.Inbound;
using AeroFlight.Domain.Ops;
using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Flights.Commands;

// Single place every flight-update source funnels through — Replace (full) and Patch (sparse) share the same rules/save/auto-link tail.
public interface IFlightUpdateService
{
    Task<UpdateFlightResult> ReplaceArrivalAsync(long id, UpdateArrivalFlightRequest request, CancellationToken ct);
    Task<UpdateFlightResult> ReplaceDepartureAsync(long id, UpdateDepartureFlightRequest request, CancellationToken ct);
    Task PatchArrivalAsync(ArrivalFlight flight, ArrivalFlightUpdated message, CancellationToken ct);
    Task PatchDepartureAsync(DepartureFlight flight, DepartureFlightUpdated message, CancellationToken ct);
}

public sealed class FlightUpdateService(
    IAeroFlightDbContext db, IRuleEngine ruleEngine, IFlightCountryTypeResolver countryTypeResolver, IFlightAutoLinker autoLinker,
    IResourceAutoCloseService resourceAutoClose)
    : IFlightUpdateService
{
    public async Task<UpdateFlightResult> ReplaceArrivalAsync(long id, UpdateArrivalFlightRequest request, CancellationToken ct)
    {
        var flight = await db.Query<ArrivalFlight>().Include(f => f.Routings).FirstOrDefaultAsync(f => f.Id == id, ct);
        if (flight is null)
            return new UpdateFlightResult(UpdateFlightOutcome.NotFound, null);

        var r = request with { FlightNumber = FlightNumberFormat.Normalize(request.FlightNumber) };
        var newFlightDate = DateOnly.FromDateTime(r.Sta.Date);

        var duplicate = await db.Query<ArrivalFlight>()
            .AnyAsync(f => f.Id != id && f.AirlineCode == flight.AirlineCode && f.FlightNumber == r.FlightNumber && f.FlightDate == newFlightDate, ct);
        if (duplicate)
            return new UpdateFlightResult(UpdateFlightOutcome.DuplicateFlight, null);

        UpdateFlightSteps.ApplyFlightNumber(flight, r.FlightNumber);
        UpdateFlightSteps.ApplyStatus(flight, r.StatusCode);
        flight.UpdateSta(r.Sta);
        flight.UpdateOrigin(r.OriginAirportCode, r.DivertAirportCode);

        var previousAircraftCode = flight.AircraftCode;
        await UpdateFlightSteps.ApplyCommonFieldsAsync(flight, countryTypeResolver, flight.Routings.Select(s => s.Code),
            new CommonFlightUpdateFields(r.CallSign, r.CodeshareNumber, r.FlightTypeCode, r.AircraftCode, r.AircraftTypeCode,
                r.OperatingAirlineCode, r.TerminalCode, r.ParkPositionCode, r.HandlingAgentCode, r.Comment,
                r.PaxTotal, r.PaxTransit, r.PaxAssistanceRequired, r.Pax1, r.Pax2, r.Pax3, r.CrewNumber, r.CrewCockpit,
                r.BaggageWeight, r.BaggageTotal, r.CargoWeight, r.MailWeight, r.TotalWeight, r.EquipmentWeight, r.CabinLuggage),
            ct);
        flight.UpdateEta(r.Eta);
        flight.RecordTouchDown(r.TouchDown);
        if (r.Ata is { } ata) flight.RecordAta(ata);
        if (r.OnBlock is { } onBlock) flight.RecordOnBlock(onBlock);

        await FinalizeArrivalAsync(flight, previousAircraftCode, ct);

        return new UpdateFlightResult(UpdateFlightOutcome.Updated, FlightResponseFactory.FromArrival(flight));
    }

    public async Task<UpdateFlightResult> ReplaceDepartureAsync(long id, UpdateDepartureFlightRequest request, CancellationToken ct)
    {
        var flight = await db.Query<DepartureFlight>().Include(f => f.Routings).FirstOrDefaultAsync(f => f.Id == id, ct);
        if (flight is null)
            return new UpdateFlightResult(UpdateFlightOutcome.NotFound, null);

        var r = request with { FlightNumber = FlightNumberFormat.Normalize(request.FlightNumber) };
        var newFlightDate = DateOnly.FromDateTime(r.Std.Date);

        var duplicate = await db.Query<DepartureFlight>()
            .AnyAsync(f => f.Id != id && f.AirlineCode == flight.AirlineCode && f.FlightNumber == r.FlightNumber && f.FlightDate == newFlightDate, ct);
        if (duplicate)
            return new UpdateFlightResult(UpdateFlightOutcome.DuplicateFlight, null);

        UpdateFlightSteps.ApplyFlightNumber(flight, r.FlightNumber);
        UpdateFlightSteps.ApplyStatus(flight, r.StatusCode);
        flight.UpdateStd(r.Std);
        flight.UpdateDestination(r.DestinationAirportCode);

        var previousAircraftCode = flight.AircraftCode;
        await UpdateFlightSteps.ApplyCommonFieldsAsync(flight, countryTypeResolver, flight.Routings.Select(s => s.Code),
            new CommonFlightUpdateFields(r.CallSign, r.CodeshareNumber, r.FlightTypeCode, r.AircraftCode, r.AircraftTypeCode,
                r.OperatingAirlineCode, r.TerminalCode, r.ParkPositionCode, r.HandlingAgentCode, r.Comment,
                r.PaxTotal, r.PaxTransit, r.PaxAssistanceRequired, r.Pax1, r.Pax2, r.Pax3, r.CrewNumber, r.CrewCockpit,
                r.BaggageWeight, r.BaggageTotal, r.CargoWeight, r.MailWeight, r.TotalWeight, r.EquipmentWeight, r.CabinLuggage),
            ct);
        flight.UpdateEtd(r.Etd);
        if (r.Atd is { } atd) flight.RecordAtd(atd);
        if (r.OffBlock is { } offBlock) flight.RecordOffBlock(offBlock);
        if (r.TakeOff is { } takeOff) flight.RecordTakeOff(takeOff);

        await FinalizeDepartureAsync(flight, previousAircraftCode, ct);

        return new UpdateFlightResult(UpdateFlightOutcome.Updated, FlightResponseFactory.FromDeparture(flight));
    }

    public async Task PatchArrivalAsync(ArrivalFlight flight, ArrivalFlightUpdated message, CancellationToken ct)
    {
        var previousAircraftCode = flight.AircraftCode;
        flight.ApplyUpdate(message);
        await FinalizeArrivalAsync(flight, previousAircraftCode, ct);
    }

    public async Task PatchDepartureAsync(DepartureFlight flight, DepartureFlightUpdated message, CancellationToken ct)
    {
        var previousAircraftCode = flight.AircraftCode;
        flight.ApplyUpdate(message);
        await FinalizeDepartureAsync(flight, previousAircraftCode, ct);
    }

    private async Task FinalizeArrivalAsync(ArrivalFlight flight, string? previousAircraftCode, CancellationToken ct)
    {
        await ruleEngine.EvaluateAsync(RuleTrigger.ArrivalUpdated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightUpdated, flight, ct);
        await resourceAutoClose.CloseAllIfCompletedAsync(flight, ct);
        await db.SaveChangesAsync(ct);

        await UpdateFlightSteps.RelinkIfAircraftChangedAsync(previousAircraftCode, flight.AircraftCode, ct,
            c => autoLinker.TryLinkArrivalAsync(flight, c));
    }

    private async Task FinalizeDepartureAsync(DepartureFlight flight, string? previousAircraftCode, CancellationToken ct)
    {
        await ruleEngine.EvaluateAsync(RuleTrigger.DepartureUpdated, flight, ct);
        await ruleEngine.EvaluateAsync(RuleTrigger.FlightUpdated, flight, ct);
        await resourceAutoClose.CloseAllIfCompletedAsync(flight, ct);
        await db.SaveChangesAsync(ct);

        await UpdateFlightSteps.RelinkIfAircraftChangedAsync(previousAircraftCode, flight.AircraftCode, ct,
            c => autoLinker.TryLinkDepartureAsync(flight, c));
    }
}

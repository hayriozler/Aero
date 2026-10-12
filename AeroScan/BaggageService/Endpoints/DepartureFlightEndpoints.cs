using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Domain.Aggregates.Flights;
using Domain.Aggregates.Reconciliations;
using Domain.Enums;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class DepartureFlightEndpoints
{
    public static IEndpointRouteBuilder MapDepartureFlightEndpoints(this IEndpointRouteBuilder app)
    {
        var flights = app.MapGroup("/api/departure/flights")
            .RequireAuthorization(Policies.AnyRole);

        // ── Departure flights ──────────────────────────────────────────────────
        flights.MapGet("", ListDepartureFlights)
            .WithName("ListDepartureFlights")
            .Produces<IReadOnlyList<DepartureFlightDto>>();

        flights.MapGet("/{id:int}", GetDepartureFlight)
            .WithName("GetDepartureFlight")
            .Produces<DepartureFlightDto>()
            .ProducesProblem(404);

        return app;
    }

    private static readonly DepartureFlightStatus[] _activeDepartureStatuses =
    [
        DepartureFlightStatus.Scheduled, DepartureFlightStatus.Estimated, DepartureFlightStatus.Delayed,
        DepartureFlightStatus.CheckInOpen, DepartureFlightStatus.Boarding,
        DepartureFlightStatus.FinalCall, DepartureFlightStatus.GateClosed
    ];

    private static IQueryable<DepartureFlightRow> WithDepartureJoins(
        this IQueryable<DepartureFlight> query, AeroScanDataContext db)
        => query
            .LeftJoin(db.DepartureFlightReconciliationSet,
                f => f.Id,
                r => r.FlightId,
                (f, r) => new { Flight = f, Recon = r })
            .LeftJoin(db.CompanySet,
                fr => fr.Flight.HandlingCompanyCode,
                c => c.Code,
                (fr, c) => DepartureFlightRow.Create(fr.Flight.Id,
                    fr.Flight.RemoteSystemId,
                    fr.Flight.AirlineCode,
                    fr.Flight.FlightNumber,
                    fr.Flight.ScheduledDateTime,
                    fr.Flight.EstimatedDateTime,
                    fr.Flight.ActualDateTime,
                    fr.Flight.FlightIataDate,
                    fr.Flight.IntDom,
                    fr.Flight.Terminal,
                    fr.Flight.Registration,
                    fr.Flight.ParkingPosition,
                    fr.Flight.TotalPassengers,
                    fr.Flight.DestinationAirport,
                    fr.Flight.NextAirport,
                    fr.Flight.Gate,
                    fr.Flight.GateStatus,
                    fr.Flight.CheckIn,
                    fr.Flight.CheckInStatus,
                    fr.Flight.Chute,
                    fr.Flight.FlightStatus,
                    fr.Recon,
                    c == null ? null : c.Code,
                    c == null ? null : c.Name,
                    c == null ? null : c.Type));
    private static DepartureFlightDto ToDto(this DepartureFlightRow row) => new(
        Id:                          row.Id,
        RemoteSystemId:              $"{row.RemoteSystemId}",
        AirlineCode:                 row.AirlineCode,
        FlightNumber:                row.FlightNumber,
        FlightIataDate:              row.FlightIataDate,
        Destination:                 row.Destination,             
        OperationDateTime:           row.OperationDateTime,
        Status:                      row.FlightStatus,
        Terminal:                    row.Terminal,
        IntDom:                      row.IntDom,
        Chute:                       row.Chute,
        CheckIn:                     row.CheckIn,
        CheckInStatus:               row.CheckInStatus,
        Gate:                        row.Gate,
        GateStatus:                  row.GateStatus,
        HandlingCompanyCode:         row.CompanyCode,
        HandlingCompanyName:         row.CompanyName,
        ExpectedBagCount:            row.Recon?.ExpectedBagCount            ?? 0,
        LoadedBagCount:              row.Recon?.LoadedBagCount              ?? 0,
        OffloadedCount:              row.Recon?.OffloadedCount              ?? 0,
        ToBeOffloadedCount:          row.Recon?.ToBeOffloadedCount          ?? 0,
        WaitingToLoadBagCount:       row.Recon?.WaitingToLoadBagCount       ?? 0,
        MissingBagCount:             row.Recon?.MissingBagCount             ?? 0,
        ReconciledBagCount:          row.Recon?.ReconciledBagCount          ?? 0,
        ForceLoadedBagCount:         row.Recon?.ForceLoadedBagCount         ?? 0,
        OnwardBagCount:              row.Recon?.OnwardBagCount              ?? 0,
        TransferLoadedBagCount:      row.Recon?.TransferLoadedBagCount      ?? 0,
        TransferMissingBagCount:     row.Recon?.TransferMissingBagCount     ?? 0,
        NotBoardedPassengerBagCount: row.Recon?.NotBoardedPassengerBagCount ?? 0,
        RushBagCount:                row.Recon?.RushBagCount                ?? 0,
        PriorityBagCount:            row.Recon?.PriorityBagCount            ?? 0);
    private static async Task<Ok<IReadOnlyList<DepartureFlightDto>>> ListDepartureFlights(
        AeroScanDataContext db,
        HttpContext httpContext,
        bool includeAll = false,
        DateOnly? from = null,
        DateOnly? to = null,
        string? airlineCode = null,
        string? flightIataDate = null,
        CancellationToken ct = default)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var fromUtc = (from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3))).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc   = (to   ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var query = db.DepartureFlightSet
            .Where(f => f.ScheduledDateTime >= fromUtc && f.ScheduledDateTime <= toUtc);

        if (!includeAll)      query = query.Where(f => _activeDepartureStatuses.Contains(f.FlightStatus));
        if (isHandlingAgent)  query = query.Where(f => f.HandlingCompanyCode == userCompanyCode);
        if (!string.IsNullOrWhiteSpace(airlineCode))  query = query.Where(f => f.AirlineCode == airlineCode.ToUpperInvariant());
        if (!string.IsNullOrEmpty(flightIataDate))    query = query.Where(f => f.FlightIataDate == flightIataDate);

        var rows = await query
            .OrderBy(f => f.ScheduledDateTime)
            .WithDepartureJoins(db)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<DepartureFlightDto>>(rows.ConvertAll(r => r.ToDto()));
    }

    private static async Task<Results<Ok<DepartureFlightDto>, NotFound>> GetDepartureFlight(
        int id,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var row = await db.DepartureFlightSet
            .Where(f => f.Id == id)
            .WithDepartureJoins(db)
            .FirstOrDefaultAsync(ct);

        if (row is null) return TypedResults.NotFound();
        if (isHandlingAgent && row.CompanyCode != userCompanyCode) return TypedResults.NotFound();

        return TypedResults.Ok(row.ToDto());
    }
    internal sealed record DepartureFlightRow(
        int Id,
        string RemoteSystemId,
        string AirlineCode,
        string FlightNumber,
        DateTime OperationDateTime,
        string FlightIataDate,
        string IntDom,
        string? Terminal,
        string? Registration,
        string? ParkingPosition,
        DepartureFlightStatus FlightStatus,
        int TotalPassengers,
        string Destination,
        string NextAirport,
        string? Gate,
        string? GateStatus,
        string? CheckIn,
        string? CheckInStatus,
        string? Chute,
        DepartureFlightReconciliation? Recon,
        string? CompanyCode,
        string? CompanyName,
        CompanyType? CompanyType)
    {
        internal static DepartureFlightRow Create(
            int id,
            string remoteSystemId,
            string airlineCode,
            string flightNumber,
            DateTime scheduledDateTime,
            DateTime? estimatedDateTime,
            DateTime? actualDateTime,
            string flightIataDate,
            string intDom,
            string? terminal,
            string? registration,
            string? parkingPosition,
            int totalPassengers,
            string destinationAirport,
            string nextAirport,
            string? gate,
            string? gateStatus,
            string? checkIn,
            string? checkInStatus,
            string? chute,
            DepartureFlightStatus flightStatus,
            DepartureFlightReconciliation? recon,
            string? companyCode,
            string? companyName,
            CompanyType? companyType) => new(
                id,
                remoteSystemId,
                airlineCode,
                flightNumber,
                actualDateTime ?? estimatedDateTime ?? scheduledDateTime,
                flightIataDate,
                intDom,
                terminal,
                registration,
                parkingPosition,
                flightStatus,
                totalPassengers,
                destinationAirport == nextAirport ? destinationAirport : $"{destinationAirport}/{nextAirport}",
                nextAirport,
                gate,
                gateStatus,
                checkIn,
                checkInStatus,
                chute,
                recon,
                companyCode,
                companyName,
                companyType);
    }
}

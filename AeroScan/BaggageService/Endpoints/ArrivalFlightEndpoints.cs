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

public static class ArrivalFlightEndpoints
{
    public static IEndpointRouteBuilder MapArrivalFlightEndpoints(this IEndpointRouteBuilder app)
    {
        var flights = app.MapGroup("/api/arrival/flights")
            .RequireAuthorization(Policies.AnyRole);

        flights.MapGet("", ListArrivalFlights)
            .WithName("ListArrivalFlights")
            .Produces<IReadOnlyList<ArrivalFlightDto>>();

        flights.MapGet("/{id:int}", GetArrivalFlight)
            .WithName("GetArrivalFlight")
            .Produces<ArrivalFlightDto>()
            .ProducesProblem(404);

        return app;
    }

    private static readonly ArrivalFlightStatus[] _activeArrivalStatuses =
    [
        ArrivalFlightStatus.Scheduled, ArrivalFlightStatus.Estimated, ArrivalFlightStatus.Landed
    ];

    private static IQueryable<ArrivalFlightRow> WithArrivalJoins(
        this IQueryable<ArrivalFlight> query, AeroScanDataContext db)
        => query
            .LeftJoin(db.ArrivalFlightReconciliationSet,
                f  => f.Id,
                r  => r.FlightId,
                (f, r) => new { Flight = f, Recon = r })
            .LeftJoin(db.CompanySet,
                fr => fr.Flight.HandlingCompanyCode,
                c  => c.Code,
                (fr, c) => ArrivalFlightRow.Create(fr.Flight.Id,
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
                    fr.Flight.PreviousAirport,
                    fr.Flight.OriginAirport,
                    fr.Flight.Carousel,
                    fr.Flight.FlightStatus,
                    fr.Recon,
                    c == null ? null : c.Code,
                    c == null ? null : c.Name,
                    c == null ? null : c.Type));

    private static ArrivalFlightDto ToDto(this ArrivalFlightRow row) => new(
        Id:                          row.Id,
        RemoteSystemId:              $"{row.RemoteSystemId}",
        AirlineCode:                 row.AirlineCode,
        FlightNumber:                row.FlightNumber,
        FlightIataDate:              row.FlightIataDate,
        Destination:                 row.Destination,
        OperationDateTime:           row.OperationDateTime,
        Status:                      row.FlightStatus,
        Carousel:                    row.Carousel,
        IntDom:                      row.IntDom,
        Terminal:                    row.Terminal,
        HandlingCompanyCode:         row.CompanyCode,
        HandlingCompanyName:         row.CompanyName,
        ExpectedBagCount:            row.Recon?.ExpectedBagCount            ?? 0,
        UnloadedBagCount:            row.Recon?.UnloadedBagCount            ?? 0,
        RemainingOnAircraftBagCount: row.Recon?.RemainingOnAircraftBagCount ?? 0,
        ToBeltBagCount:              row.Recon?.ToBeltBagCount              ?? 0,
        DeliveredBagCount:           row.Recon?.DeliveredBagCount           ?? 0,
        TransferBagCount:            row.Recon?.TransferBagCount            ?? 0,
        MissingBagCount:             row.Recon?.MissingBagCount             ?? 0,
        UnknownBagCount:             row.Recon?.UnknownBagCount             ?? 0,
        RushBagCount:                row.Recon?.RushBagCount                ?? 0);

    private static async Task<Ok<IReadOnlyList<ArrivalFlightDto>>> ListArrivalFlights(
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

        var query = db.ArrivalFlightSet
            .Where(f => f.ScheduledDateTime >= fromUtc && f.ScheduledDateTime <= toUtc);

        if (!includeAll)      query = query.Where(f => _activeArrivalStatuses.Contains(f.FlightStatus));
        if (isHandlingAgent)  query = query.Where(f => f.HandlingCompanyCode == userCompanyCode);
        if (!string.IsNullOrWhiteSpace(airlineCode))  query = query.Where(f => f.AirlineCode == airlineCode.ToUpperInvariant());
        if (!string.IsNullOrEmpty(flightIataDate))    query = query.Where(f => f.FlightIataDate == flightIataDate);

        var rows = await query
            .OrderBy(f => f.ScheduledDateTime)
            .WithArrivalJoins(db)
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<ArrivalFlightDto>>(rows.ConvertAll(r => r.ToDto()));
    }

    private static async Task<Results<Ok<ArrivalFlightDto>, NotFound>> GetArrivalFlight(
        int id,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userCompanyCode = httpContext.GetCompanyCode();
        var isHandlingAgent = httpContext.IsHandlingAgent();

        var row = await db.ArrivalFlightSet
            .Where(f => f.Id == id)
            .WithArrivalJoins(db)
            .FirstOrDefaultAsync(ct);

        if (row is null) return TypedResults.NotFound();
        if (isHandlingAgent && row.CompanyCode != userCompanyCode)
            return TypedResults.NotFound();

        return TypedResults.Ok(row.ToDto());
    }

    internal sealed record ArrivalFlightRow(
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
        ArrivalFlightStatus FlightStatus,
        int TotalPassengers,
        string Destination,
        string? Carousel,
        ArrivalFlightReconciliation? Recon,
        string? CompanyCode,
        string? CompanyName,
        CompanyType? CompanyType)
    {
        internal static ArrivalFlightRow Create(
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
            string previousAirport,
            string originAirport,
            string? carousel,
            ArrivalFlightStatus flightStatus,
            ArrivalFlightReconciliation? recon,
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
                originAirport == previousAirport ? originAirport : $"{originAirport}/{previousAirport}",
                carousel,
                recon,
                companyCode,
                companyName,
                companyType);
    }
}

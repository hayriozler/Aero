using BaggageService.Persistence;
using Contracts.Consts;
using Contracts.Dtos;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class DashboardEndpoints
{
    private static readonly DepartureFlightStatus[] _activeStatuses =
    [
        DepartureFlightStatus.CheckInOpen,
        DepartureFlightStatus.Boarding,
        DepartureFlightStatus.FinalCall
    ];

    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var dashboard = app.MapGroup("/api/dashboard")
            .RequireAuthorization(Policies.AirportOrRamp);

        dashboard.MapGet("/summary", GetSummary)
            .WithName("GetDashboardSummary")
            .Produces<DashboardSummaryDto>()
            ;

        return app;
    }

    private static async Task<Ok<DashboardSummaryDto>> GetSummary(
        AeroScanDataContext db,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;

        var activeFlights = await db.DepartureFlightSet
            .CountAsync(f => _activeStatuses.Contains(f.FlightStatus), ct);
        var bagsScannedToday = 0;
        //var bagsScannedToday = await db.DepartureBagSet
        //    .Join(db.FlightPassengerSet,
        //        bag => bag.FlightPassengerId,
        //        flight => flight.Id,
        //        (bag, flight) => new { bag, flight })
        //    .CountAsync(b => b.bag.UpdatedAt >= today
        //        && _activeStatuses.Contains(b..FlightStatus), ct);

        var reconErrors = await db.DepartureBagSet
            .CountAsync(b => b.DepartureBaggageStatus == DepartureBaggageStatus.Unknown, ct);

        var flights = await db.DepartureFlightSet
            .Where(f => _activeStatuses.Contains(f.FlightStatus))
            .OrderBy(f => f.ScheduledDateTime)
            .Take(10)
            .Select(f => new
            {
                f.AirlineCode,
                f.FlightNumber,
                f.NextAirport,
                f.DestinationAirport,
                f.ScheduledDateTime,
                f.EstimatedDateTime,
                f.FlightStatus,
                f.Gate,
                f.CheckIn,
                f.Terminal,
                Recon = f.ReconciliationRecords
                    .OrderByDescending(r => r.SnapshotAt)
                    .Select(r => new
                    {
                        r.ExpectedBagCount,
                        r.LoadedBagCount,
                        r.MissingBagCount,
                        r.OffloadedCount,
                        r.RushBagCount
                    })
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var flightRows = flights.Select(f =>
        {
            var expected  = f.Recon?.ExpectedBagCount ?? 0;
            var loaded    = f.Recon?.LoadedBagCount   ?? 0;
            var missing   = f.Recon?.MissingBagCount  ?? 0;
            var offloaded = f.Recon?.OffloadedCount   ?? 0;
            var rush      = f.Recon?.RushBagCount     ?? 0;
            var pct       = expected > 0 ? (int)Math.Round(loaded * 100.0 / expected) : 0;

            return new FlightReconciliationRowDto(
                FlightNo:            f.AirlineCode + f.FlightNumber,
                ScheduledDeparture:  f.ScheduledDateTime.ToString("HH:mm"),
                EstimatedDeparture:  f.EstimatedDateTime?.ToString("HH:mm"),
                Gate:                f.Gate,
                CheckIn:             f.CheckIn,
                Terminal:            f.Terminal,
                FlightStatus:        f.FlightStatus,
                ExpectedBagCount:    expected,
                LoadedBagCount:      loaded,
                MissingBagCount:     missing,
                OffloadedCount:      offloaded,
                RushBagCount:        rush,
                ReconciliationPercent: pct,
                HasMismatch:         missing > 0,
                NextAirport:         f.NextAirport,
                DestinationAirport:  f.DestinationAirport);
        }).ToList();

        var throughput = await db.DepartureBagSet
            .CountAsync(b => b.UpdatedAt >= DateTime.UtcNow.AddMinutes(-1), ct);

        return TypedResults.Ok(new DashboardSummaryDto(
            ActiveFlights:         activeFlights,
            TotalBagsScannedToday: bagsScannedToday,
            ReconciliationErrors:  reconErrors,
            ThroughputPerMinute:   throughput,
            FlightReconciliations: flightRows));
    }
}

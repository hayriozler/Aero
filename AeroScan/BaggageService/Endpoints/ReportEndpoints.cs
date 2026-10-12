using BaggageService.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;


public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var reports = app.MapGroup("/api/reports").RequireAuthorization();

        reports.MapGet("/flight/{flightKey}", GetFlightReport)
            .WithName("GetFlightReport")
            .Produces<FlightReportDto>()
            .ProducesProblem(404);

        reports.MapGet("/daily", GetDailyStats)
            .WithName("GetDailyStats")
            .Produces<DailyStatsDto>();

        reports.MapGet("/mishandled", GetMishandledBags)
            .WithName("GetMishandledBags")
            .Produces<IReadOnlyList<MishandledBagDto>>();

        return app;
    }

    private static async Task<Results<Ok<IReadOnlyList<FlightReportDto>>, NotFound>> GetFlightReport(
        string flightKey,
        AeroScanDataContext dbContext,
        CancellationToken ct)
    {
        var report = await dbContext.Database.SqlQuery<FlightReportDto>($"""
            SELECT r.flight_key AS FlightKey,
                   r.total_checked AS TotalChecked,
                   r.total_loaded AS TotalLoaded,
                   r.total_offloaded AS TotalOffloaded,
                   r.total_transferred AS TotalTransferred,
                   r.status AS Status,
                   r.closed_at AS ClosedAt
            FROM baggage.reconciliation_records r
            WHERE r.flight_key = {flightKey}
            """
           ).ToListAsync(ct);
        return report is null ? TypedResults.NotFound() : TypedResults.Ok<IReadOnlyList<FlightReportDto>>(report);
    }

    private static async Task<Ok<DailyStatsDto>> GetDailyStats(
        DateOnly? date,
        AeroScanDataContext dbContext,
        CancellationToken ct)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var stats = await dbContext.Database.SqlQuery<DailyStatsDto>($"""
            SELECT COUNT(DISTINCT r.flight_key) AS TotalFlights,
                   SUM(r.total_checked)         AS TotalBagsChecked,
                   SUM(r.total_loaded)          AS TotalBagsLoaded,
                   SUM(r.total_offloaded)       AS TotalBagsOffloaded
            FROM baggage.reconciliation_records r
            JOIN flight.flights f ON f.key = r.flight_key
            WHERE f.operational_date = {targetDate}
            """).FirstOrDefaultAsync(ct);

        return TypedResults.Ok(stats ?? new DailyStatsDto(0, 0, 0, 0));
    }

    private static async Task<Ok<IReadOnlyList<MishandledBagDto>>> GetMishandledBags(
        DateOnly? date,
        AeroScanDataContext dbContext,
        CancellationToken ct)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var bags = await dbContext.Database.SqlQuery<MishandledBagDto>($"""
            SELECT b.tag_number AS TagNumber,
                   b.flight_key AS FlightKey,
                   b.status AS Status,
                   b.updated_at AS UpdatedAt
            FROM baggage.bags b
            JOIN flight.flights f ON f.key = b.flight_key
            WHERE f.operational_date = {targetDate}
              AND b.status IN ('Offloaded', 'Lost')
            ORDER BY b.updated_at DESC
            """).ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<MishandledBagDto>>(bags);
    }
}

public sealed record FlightReportDto(
    string FlightKey, int TotalChecked, int TotalLoaded,
    int TotalOffloaded, int TotalTransferred, string Status, DateTime? ClosedAt);

public sealed record DailyStatsDto(
    int TotalFlights, int TotalBagsChecked, int TotalBagsLoaded, int TotalBagsOffloaded);

public sealed record MishandledBagDto(
    string TagNumber, string FlightKey, string Status, DateTime UpdatedAt);
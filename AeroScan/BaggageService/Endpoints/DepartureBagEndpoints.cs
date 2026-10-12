using BaggageService.Persistence;
using Contracts.Dtos;
using Domain.Aggregates.Bags;
using Domain.Aggregates.Flights;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Endpoints;

public static class DepartureBagEndpoints
{
    public static IEndpointRouteBuilder MapDepartureBagEndpoints(this IEndpointRouteBuilder app)
    {
        var bags = app.MapGroup("/api/departure/bags").RequireAuthorization();

        bags.MapGet("/{tagNumber}", GetBagByTag)
            .WithName("GetBagByTag")
            .Produces<DepartureBagDto>()
            .ProducesProblem(404);

        bags.MapGet("/", GetBagsForFlight)
            .WithName("GetBagsForFlight")
            .Produces<IReadOnlyList<DepartureBagDto>>();

        //bags.MapPost("/checkin", CheckInBag)
        //    .WithName("CheckInBag")
        //    .AddEndpointFilter<ValidationFilter<CheckInBagRequest>>()
        //    .Produces<BagDto>(201)
        //    .ProducesProblem(400)
        //    .RequireAuthorization(Policies.BaggageAgent);

        //bags.MapPost("/{tagNumber}/offload", OffloadBag)
        //    .WithName("OffloadBag")
        //    .AddEndpointFilter<ValidationFilter<OffloadBagRequest>>()
        //    .Produces<BagDto>()
        //    .ProducesProblem(400)
        //    .ProducesProblem(404)
        //    .RequireAuthorization(Policies.BaggageAgent);

        //bags.MapPost("/transfer", TransferBag)
        //    .WithName("TransferBag")
        //    .AddEndpointFilter<ValidationFilter<TransferBagRequest>>()
        //    .Produces<BagDto>()
        //    .ProducesProblem(400)
        //    .ProducesProblem(404)
        //    .RequireAuthorization(Policies.BaggageAgent);

        //bags.MapGet("/{tagNumber}/journey", GetBagJourney)
        //    .WithName("GetBagJourney")
        //    .Produces<IReadOnlyList<ScanEventDto>>()
        //    .ProducesProblem(404);

        //bags.MapGet("/unmatched", GetUnmatchedBags)
        //    .WithName("GetUnmatchedBags")
        //    .Produces<IReadOnlyList<BagDto>>();

        return app;
    }

    private static async Task<Results<Ok<DepartureBagDto>, NotFound>> GetBagByTag(
        string tagNumber,
        AeroScanDataContext db,
        CancellationToken ct)
    {
        var bag = await db.DepartureBagSet
            
            .FirstOrDefaultAsync(b => b.TagNumber == tagNumber, ct);

        return bag is null ? TypedResults.NotFound() : TypedResults.Ok(bag.ToDto());
    }

    private static async Task<Ok<IReadOnlyList<DepartureBagDto>>> GetBagsForFlight(
        int flightId,
        AeroScanDataContext db,
        CancellationToken ct)
    {
        //var bags = await db.DepartureBagSet
        //    .Where(b => b.FlightId == flightId)
        //    .ToListAsync(ct);

        var bags = await db.DepartureFlightPassengerSet
            .Where(b => b.FlightId == flightId)
            .Join(db.DepartureBagSet, fp => fp.Id, b => b.FlightPassengerId, (fp, b) => 
            new DepartureBagDto(b.Id, b.FlightPassengerId, b.TagNumber, b.FlightPassenger.Flight.AirlineCode,
            b.FlightPassenger.Flight.FlightNumber, b.FlightPassenger.Flight.FlightIataDate, b.FlightPassenger.Destination,
            b.FlightPassenger.SecurityNumber, b.FlightPassenger.SequenceNumber, b.FlightPassenger.PassengerName,
            b.DepartureBaggageStatus, b.CheckedWeight, b.Class, b.IsDeleted))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<DepartureBagDto>>(bags);
    }

    //private static async Task<Results<Created<BagDto>, BadRequest<string>>> CheckInBag(
    //    CheckInBagRequest request,
    //    AeroScanDataContext db,
    //    CancellationToken ct)
    //{
    //    BagTagNumber.TryParse(request.TagNumber, out var tag);

    //    //if (!FlightKey.TryParse(request.FlightKey, out var flightKey))
    //    //    return TypedResults.BadRequest("Invalid flight key format.");

    //    var existing = await db.BagSet.FirstOrDefaultAsync(b => b.TagNumber == tag, ct);

    //    DepartureBag bag;
    //    if (existing is null)
    //    {
    //        bag = DepartureBag.Create(
    //            tag,
    //            Weight.FromKilograms(request.WeightKg),
    //            request.Class,
    //            request.PassengerId);
    //        db.BagSet.Add(bag);
    //    }
    //    else
    //    {
    //        bag = existing;
    //    }

    //    var result = bag.CheckIn(
    //        request.OperatorId,
    //        request.DeviceId,
    //        new Location(request.LocationCode),
    //        DateTime.UtcNow);

    //    if (result.IsFailure)
    //        return TypedResults.BadRequest(result.Error!);

    //    await db.SaveChangesAsync(ct);

    //    return TypedResults.Created($"/api/bags/{tag}", bag.ToDto());
    //}

    //private static async Task<Results<Ok<BagDto>, NotFound, BadRequest<string>>> OffloadBag(
    //    string tagNumber,
    //    OffloadBagRequest request,
    //    AeroScanDataContext db,
    //    CancellationToken ct)
    //{
    //    if (!BagTagNumber.TryParse(tagNumber, out var tag))
    //        return TypedResults.NotFound();

    //    var bag = await db.BagSet.FirstOrDefaultAsync(b => b.TagNumber == tag, ct);
    //    if (bag is null) return TypedResults.NotFound();

    //    var result = bag.Offload(
    //        request.Reason,
    //        request.OperatorId,
    //        request.DeviceId,
    //        new Location(request.LocationCode),
    //        DateTime.UtcNow);

    //    if (result.IsFailure)
    //        return TypedResults.BadRequest(result.Error!);

    //    await db.SaveChangesAsync(ct);

    //    return TypedResults.Ok(bag.ToDto());
    //}

    //private static async Task<Results<Ok<BagDto>, NotFound, BadRequest<string>>> TransferBag(
    //    TransferBagRequest request,
    //    AeroScanDataContext db,
    //    CancellationToken ct)
    //{
    //    //if (!BagTagNumber.TryParse(request.TagNumber, out var tag))
    //    //    return TypedResults.NotFound();

    //    //var bag = await db.BagSet.FirstOrDefaultAsync(b => b.TagNumber == tag, ct);
    //    //if (bag is null) return TypedResults.NotFound();

    //    //var result = bag.Transfer(targetFlight);
    //    //if (result.IsFailure)
    //    //    return TypedResults.BadRequest(result.Error!);

    //    //await db.SaveChangesAsync(ct);

    //    return TypedResults.NotFound();
    //}

    //private static async Task<Results<Ok<IReadOnlyList<ScanEventDto>>, NotFound>> GetBagJourney(
    //    string tagNumber,
    //    AeroScanDataContext db,
    //    CancellationToken ct)
    //{
    //    if (!BagTagNumber.TryParse(tagNumber, out var tag))
    //        return TypedResults.NotFound();

    //    var bag = await db.BagSet
    //        .Include(n=>n.ScanHistory)
    //        .FirstOrDefaultAsync(b => b.TagNumber == tag, ct);

    //    if (bag is null) return TypedResults.NotFound();

    //    return TypedResults.Ok<IReadOnlyList<ScanEventDto>>(
    //        [.. bag.ScanHistory.Select(s => s.ToDto())]);
    //}

    //private static async Task<Ok<IReadOnlyList<BagDto>>> GetUnmatchedBags(
    //    string flightKey,
    //    AeroScanDataContext db,
    //    CancellationToken ct)
    //{
    //    //if (!FlightKey.TryParse(flightKey, out var key))
    //    //    return TypedResults.Ok<IReadOnlyList<BagDto>>([]);

    //    //var bags = await db.BagSet
    //    //    .Where(b => b.FlightKey == key && b.PassengerId == null)
    //    //    .ToListAsync(ct);

    //    //return TypedResults.Ok<IReadOnlyList<BagDto>>([.. bags.Select(b => b.ToDto())]);

    //    return TypedResults.Ok<IReadOnlyList<BagDto>>([]);
    //}    
    private static DepartureBagDto ToDto(this DepartureBag departureBag) =>
    new(departureBag.Id, departureBag.FlightPassengerId, 
        departureBag.TagNumber, departureBag.FlightPassenger.Flight.AirlineCode, 
        departureBag.FlightPassenger.Flight.FlightNumber, 
        departureBag.FlightPassenger.Flight.FlightIataDate, 
        departureBag.FlightPassenger.Destination,
        departureBag.FlightPassenger.SecurityNumber, departureBag.FlightPassenger.SequenceNumber, 
        departureBag.FlightPassenger.PassengerName,
        departureBag.DepartureBaggageStatus, 
        departureBag.CheckedWeight, 
        departureBag.Class, 
        departureBag.IsDeleted
        );
}


using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Responses;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Endpoints;

public static class ParsedMessageEndpoints
{
    private const int _defaultLimit = 100;
    private const int _maxLimit = 500;

    public static IEndpointRouteBuilder MapParsedMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/parsedmessages").WithTags("ParsedMessages");

        group.MapGet("/", async (
            TextMessageParserDbContext db,
            CancellationToken cancellationToken,
            int? limit,
            long? afterId) =>
        {
            var take = Math.Clamp(limit ?? _defaultLimit, 1, _maxLimit);
            var rows = await db.ParsedMessages
                .AsNoTracking()
                .AsSplitQuery()
                .Include(m => m.BagTags)
                .Include(m => m.Onwards)
                .Where(m => !m.IsConsumed && m.Id > (afterId ?? 0))
                .OrderBy(m => m.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
            return Results.Ok(rows.Select(ToResponse).ToList());
        })
        .WithName("GetUnconsumedParsedMessages")
        .WithSummary("Returns parsed messages that no consumer has acknowledged yet")
        .WithDescription("Ordered by id, at most 500 per call (default 100). Reading does not change the records; acknowledge them with POST /api/parsedmessages/consumed. Use afterId to page forward without acknowledging.")
        .Produces<List<ParsedMessageResponse>>()
        .AllowAnonymous();

        group.MapPost("/consumed", async (
            ConsumedRequest request,
            TextMessageParserDbContext db,
            CancellationToken cancellationToken) =>
        {
            if (request.Ids is not { Count: > 0 } || request.Ids.Count > _maxLimit)
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Ids are not valid",
                    detail: $"Send between 1 and {_maxLimit} ids");

            var now = DateTime.UtcNow;
            var ids = request.Ids.Distinct().ToList();
            var updated = await db.ParsedMessages
                .Where(m => ids.Contains(m.Id) && !m.IsConsumed)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.IsConsumed, true)
                    .SetProperty(m => m.ConsumedAt, (DateTime?)now),
                    cancellationToken);
            return Results.Ok(new ConsumedResponse(updated));
        })
        .WithName("AcknowledgeParsedMessages")
        .WithSummary("Marks parsed messages as consumed")
        .WithDescription("Already consumed or unknown ids are ignored. The response tells how many records were changed.")
        .Produces<ConsumedResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous();

        return app;
    }

    private static ParsedMessageResponse ToResponse(ParsedMessage m) => new(
        m.Id, m.TextMessageId, m.MessageType, m.AckRequest, m.SourceIndicator, m.SourceAirport,
        m.AirlineCode, m.FlightNumber, m.FlightDate, m.Destination,
        m.AuthorityToLoad, m.AuthorityToTransport, m.BagTagStatus,
        m.BagTags.Select(t => t.BagTag).ToList(),
        m.Onwards.Select(o => new ParsedOnwardFlightResponse(o.AirlineCode, o.FlightNumber, o.FlightDate, o.Destination, o.Class)).ToList(),
        m.PassengerName is null && m.PassengerStatus is null
            ? null
            : new ParsedPassengerResponse(m.PassengerStatus, m.PassengerName, m.PassengerSequenceNumber,
                m.PassengerSecurityNumber, m.PassengerClass, m.PassengerProfileStatus, m.SeatNumber),
        m.WeightUnit is null && m.WeightIndicator is null
            ? null
            : new ParsedBaggageWeightResponse(m.WeightIndicator, m.CheckedBagCount, m.CheckedWeight,
                m.UncheckedWeight, m.WeightUnit, m.BagLength, m.BagWidth, m.BagHeight, m.BaggageTypeCode),
        m.CreatedAtUtc);
}

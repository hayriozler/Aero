using Aero.Scanner.Domain.Interfaces;
using Aero.Scanner.Shared.Requests.TextMessages;
using Aero.Shared.Infrastructure.Services;

namespace Aero.Scanner.Api.Services.BaggageMessagesHandlers;

public sealed class BaggageBsmMessageServiceHandler(LoggerService<BaggageBsmMessageServiceHandler> logger)
    : IBaggageMessageHandler<TextMessageDepartureBagRequest>
{
    //  private readonly DepartureFlightStatus[] _validStatuses =
    //[
    //    DepartureFlightStatus.Scheduled,
    //    DepartureFlightStatus.CheckInOpen,
    //    DepartureFlightStatus.Boarding,
    //    DepartureFlightStatus.Delayed,
    //    DepartureFlightStatus.FinalCall
    //];
    public Task HandleAsync(
        TextMessageDepartureBagRequest dto,
        CancellationToken cancellationToken)
    {
        logger.LogWarning("BSM handler not yet implemented for {AirlineCode}{FlightNumber}", dto.AirlineCode, dto.FlightNumber);
        foreach (var tag in dto.BagTags)
        {
            logger.LogWarning("BSM handler not yet implemented for bag {TagNumber}", tag);
        }
        return Task.CompletedTask;

        //var flightId = await dbContext.DepartureFlightSet
        //    .Where(f => f.AirlineCode    == dto.AirlineCode
        //             && f.FlightNumber   == dto.FlightNumber
        //             && f.FlightIataDate == dto.FlightIataDate
        //             && _validStatuses.Contains(f.FlightStatus))
        //    .Select(f => f.Id)
        //    .FirstOrDefaultAsync(cancellationToken);

        //if (flightId == default)
        //{
        //    dbContext.TextMessageErrorSet.Add(TextMessageError.Create(
        //        messageId,
        //        ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT,
        //        $"No flight found for {dto.AirlineCode}{dto.FlightNumber}/{dto.FlightIataDate}"));
        //    logger.LogWarning(
        //        "BSM discarded — no flight found for {AirlineCode}{FlightNumber}/{FlightIataDate}",
        //        dto.AirlineCode, dto.FlightNumber, dto.FlightIataDate);
        //    return;
        //}

        //if (!dto.AuthorityToLoad)
        //{
        //    await RevokeAuthorityAsync(dbContext, messageId, flightId, dto, cancellationToken);
        //    return;
        //}

        //var flightPassengerId = await ResolveOrCreatePassengerAsync(dbContext, flightId, dto, cancellationToken);
        //var bagClass = await ResolveBaggageClassAsync(dbContext, dto.AirlineCode, dto.PassengerClass, cancellationToken);

        //var destination = dto.Onwards.Count!=0 ? dto.Onwards.Last().Destination : dto.Destination;
        //var isTransfer = dto.Onwards.Count!=0;

        //var existingTags = await dbContext.DepartureBagSet
        //    .Where(b => b.FlightPassengerId == flightPassengerId && dto.BagTags.Contains(b.TagNumber))
        //    .Select(b => b.TagNumber)
        //    .ToHashSetAsync(cancellationToken);

        //foreach (var tag in dto.BagTags)
        //{
        //    if (existingTags.Contains(tag))
        //    {
        //        logger.LogWarning("BSM NEW received for existing bag {TagNumber} on flight {FlightId}, skipping", tag, flightId);
        //        continue;
        //    }

        //    var bag = DepartureBag.Create(
        //        flightPassengerId,
        //        dto.AckRequest,
        //        dto.AuthorityToLoad,
        //        dto.AuthorityToTransport,
        //        dto.SourceIndicator,
        //        dto.SourceAirportCode,
        //        tag,
        //        destination,
        //        isTransfer,
        //        bagClass);

        //    dbContext.DepartureBagSet.Add(bag);
        //    dbContext.DepartureBagEventSet.Add(DepartureBagEvent.Create(bag,
        //        messageId,
        //        "NEW_BAG_MSG_RECIEVED"));
        //}
    }

    //private async Task<int> ResolveOrCreatePassengerAsync(
    //    TextMessageParserDbContext dbContext,
    //    int flightId,
    //    TextMessageDepartureBagDto dto,
    //    CancellationToken cancellationToken)
    //{
    //    //var baseQuery = dbContext.DepartureFlightPassengerSet.Where(p => p.FlightId == flightId);
    //    //var query = ApplyPassengerFilter(baseQuery, dto);

    //    //var existingId = await query.Select(p => p.Id).FirstOrDefaultAsync(cancellationToken);
    //    //if (existingId != 0) return existingId;
    //    //var destination = dto.Onwards.Count!=0 ? dto.Onwards.Last().Destination : dto.Destination;
    //    //var passenger = DepartureFlightPassenger.Create(
    //    //    flightId,
    //    //    dto.PassengerName,
    //    //    dto.PassengerSecurityNumber,
    //    //    dto.PassengerSequenceNumber,
    //    //    destination,
    //    //    dto.Onwards.Count!=0,
    //    //    dto.SeatNumber);

    //    //dbContext.DepartureFlightPassengerSet.Add(passenger);

    //    //await dbContext.SaveChangesAsync(cancellationToken);

    //    //return passenger.Id;
    //}

    //private async Task RevokeAuthorityAsync(
    //    TextMessageParserDbContext dbContext,
    //    long messageId,
    //    int flightId,
    //    TextMessageDepartureBagDto dto,
    //    CancellationToken cancellationToken)
    //{
    //    //var baseQuery = dbContext.DepartureFlightPassengerSet.Where(p => p.FlightId == flightId);
    //    //var query = ApplyPassengerFilter(baseQuery, dto);

    //    //var passengerId = await query.Select(p => p.Id).FirstOrDefaultAsync(cancellationToken);
    //    //if (passengerId == 0)
    //    //{
    //    //    logger.LogWarning(
    //    //        "Authority revoked but passenger not found on {AirlineCode}{FlightNumber}/{FlightIataDate}",
    //    //        dto.AirlineCode, dto.FlightNumber, dto.FlightIataDate);
    //    //    return;
    //    //}

    //    //var bagsToRevoke = await dbContext.DepartureBagSet
    //    //    .Where(b => b.FlightPassengerId == passengerId && dto.BagTags.Contains(b.TagNumber))
    //    //    .ToListAsync(cancellationToken);

    //    //foreach (var bag in bagsToRevoke)
    //    //{
    //    //    bag.SetDelete();
    //    //    dbContext.DepartureBagEventSet.Add(DepartureBagEvent.Create(bag, messageId, "AUTHORITY_REVOKED_MSG_RECEIVED"));
    //    //}
    //}

    private static IQueryable<IFlightPassenger> ApplyPassengerFilter(
        IQueryable<IFlightPassenger> query,
        TextMessageDepartureBagRequest dto)
    {
        if (dto.PassengerSecurityNumber is not null)
            return query.Where(p => p.SecurityNumber == dto.PassengerSecurityNumber);

        if (dto.PassengerSequenceNumber is not null)
            return query.Where(p => p.SequenceNumber == dto.PassengerSequenceNumber);

        return query.Where(p => p.PassengerName == dto.PassengerName && p.Destination == dto.Destination);
    }

    //private static async Task<BaggageClass> ResolveBaggageClassAsync(
    //    TextMessageParserDbContext dbContext,
    //    string airlineCode,
    //    char? rawClass,
    //    CancellationToken cancellationToken)
    //{
    //    //char effectiveClass = rawClass ?? 'Y';

    //    //char? mappedClass = await dbContext.AirlineClassMapSet
    //    //    .Where(m => m.AirlineCode == airlineCode && m.SourceClass == effectiveClass)
    //    //    .Select(m => m.TargetClass)
    //    //    .FirstOrDefaultAsync(cancellationToken);

    //    //return (mappedClass ?? effectiveClass) switch
    //    //{
    //    //    'F' or 'A' or 'P' => BaggageClass.First,
    //    //    'C' or 'B' or 'J' or 'D' => BaggageClass.Business,
    //    //    _ => BaggageClass.Economy
    //    //};
    //}
}

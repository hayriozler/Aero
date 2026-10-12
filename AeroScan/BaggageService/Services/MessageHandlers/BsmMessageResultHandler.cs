using BaggageService.Persistence;
using Contracts.Dtos;
using Domain.Aggregates.Bags;
using Domain.Enums;
using Domain.Interfaces;
using IataText.Parser.Contracts;
using IataText.Parser.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BaggageService.Services.MessageHandlers;

public sealed class BsmMessageResultHandler(LoggerService<BsmMessageResultHandler> logger)
    : IMessageResultHandler<TextMessageDepartureBagDto>
{
    private readonly DepartureFlightStatus[] _validStatuses =
  [
      DepartureFlightStatus.Scheduled,
      DepartureFlightStatus.CheckInOpen,
      DepartureFlightStatus.Boarding,
      DepartureFlightStatus.Delayed,
      DepartureFlightStatus.FinalCall
  ];
    public async Task HandleAsync(
        long messageId,
        TextMessageDepartureBagDto dto,
        IDataContext dbContext,
        CancellationToken cancellationToken)
    {
        var ctx = (AeroScanDataContext)dbContext;

        var flightId = await ctx.DepartureFlightSet
            .Where(f => f.AirlineCode    == dto.AirlineCode
                     && f.FlightNumber   == dto.FlightNumber
                     && f.FlightIataDate == dto.FlightIataDate
                     && _validStatuses.Contains(f.FlightStatus))
            .Select(f => f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (flightId == default)
        {
            ctx.TextMessageErrorSet.Add(TextMessageError.Create(
                messageId,
                ErrorCodes.OUTBOUND_FLIGHT_NOT_CORRECT,
                $"No flight found for {dto.AirlineCode}{dto.FlightNumber}/{dto.FlightIataDate}"));
            logger.LogWarning(
                "BSM discarded — no flight found for {AirlineCode}{FlightNumber}/{FlightIataDate}",
                dto.AirlineCode, dto.FlightNumber, dto.FlightIataDate);
            return;
        }

        if (!dto.AuthorityToLoad)
        {
            await RevokeAuthorityAsync(ctx, messageId, flightId, dto, cancellationToken);
            return;
        }

        var flightPassengerId = await ResolveOrCreatePassengerAsync(ctx, flightId, dto, cancellationToken);
        var bagClass          = await ResolveBaggageClassAsync(ctx, dto.AirlineCode, dto.PassengerClass, cancellationToken);

        var destination = dto.Onwards.Any() ? dto.Onwards.Last().Destination : dto.Destination;
        var isTransfer  = dto.Onwards.Any();

        var existingTags = await ctx.DepartureBagSet
            .Where(b => b.FlightPassengerId == flightPassengerId && dto.BagTags.Contains(b.TagNumber))
            .Select(b => b.TagNumber)
            .ToHashSetAsync(cancellationToken);

        foreach (var tag in dto.BagTags)
        {
            if (existingTags.Contains(tag))
            {
                logger.LogWarning("BSM NEW received for existing bag {TagNumber} on flight {FlightId}, skipping", tag, flightId);
                continue;
            }

            var bag = DepartureBag.Create(
                flightPassengerId,
                dto.AckRequest,
                dto.AuthorityToLoad,
                dto.AuthorityToTransport,
                dto.SourceIndicator,
                dto.SourceAirportCode,
                tag,
                destination,
                isTransfer,
                bagClass);

            ctx.DepartureBagSet.Add(bag);
            ctx.DepartureBagEventSet.Add(DepartureBagEvent.Create(bag,
                messageId,
                "NEW_BAG_MSG_RECIEVED"));
        }
    }

    private async Task<int> ResolveOrCreatePassengerAsync(
        AeroScanDataContext ctx,
        int flightId,
        TextMessageDepartureBagDto dto,
        CancellationToken cancellationToken)
    {
        var baseQuery = ctx.DepartureFlightPassengerSet.Where(p => p.FlightId == flightId);
        var query     = ApplyPassengerFilter(baseQuery, dto);

        var existingId = await query.Select(p => p.Id).FirstOrDefaultAsync(cancellationToken);
        if (existingId != 0) return existingId;
        var destination = dto.Onwards.Any() ? dto.Onwards.Last().Destination : dto.Destination;
        var passenger = DepartureFlightPassenger.Create(
            flightId,
            dto.PassengerName,
            dto.PassengerSecurityNumber,
            dto.PassengerSequenceNumber,
            destination,
            dto.Onwards.Any(),
            dto.SeatNumber);

        ctx.DepartureFlightPassengerSet.Add(passenger);

        await ctx.SaveChangesAsync(cancellationToken);

        return passenger.Id;
    }

    private async Task RevokeAuthorityAsync(
        AeroScanDataContext ctx,
        long messageId,
        int flightId,
        TextMessageDepartureBagDto dto,
        CancellationToken cancellationToken)
    {
        var baseQuery = ctx.DepartureFlightPassengerSet.Where(p => p.FlightId == flightId);
        var query     = ApplyPassengerFilter(baseQuery, dto);
        
        var passengerId = await query.Select(p => p.Id).FirstOrDefaultAsync(cancellationToken);
        if (passengerId == 0)
        {
            logger.LogWarning(
                "Authority revoked but passenger not found on {AirlineCode}{FlightNumber}/{FlightIataDate}",
                dto.AirlineCode, dto.FlightNumber, dto.FlightIataDate);
            return;
        }

        var bagsToRevoke = await ctx.DepartureBagSet
            .Where(b => b.FlightPassengerId == passengerId && dto.BagTags.Contains(b.TagNumber))
            .ToListAsync(cancellationToken);

        foreach (var bag in bagsToRevoke)
        {
            bag.SetDelete();
            ctx.DepartureBagEventSet.Add(DepartureBagEvent.Create(bag, messageId, "AUTHORITY_REVOKED_MSG_RECEIVED"));
        }
    }

    private static IQueryable<IFlightPassenger> ApplyPassengerFilter(
        IQueryable<IFlightPassenger> query,
        TextMessageDepartureBagDto dto)
    {
        if (dto.PassengerSecurityNumber is not null)
            return query.Where(p => p.SecurityNumber == dto.PassengerSecurityNumber);

        if (dto.PassengerSequenceNumber is not null)
            return query.Where(p => p.SequenceNumber == dto.PassengerSequenceNumber);

        return query.Where(p => p.PassengerName == dto.PassengerName && p.Destination == dto.Destination);
    }

    private static async Task<BaggageClass> ResolveBaggageClassAsync(
        AeroScanDataContext ctx,
        string airlineCode,
        char? rawClass,
        CancellationToken cancellationToken)
    {
        char effectiveClass = rawClass ?? 'Y';

        char? mappedClass = await ctx.AirlineClassMapSet
            .Where(m => m.AirlineCode == airlineCode && m.SourceClass == effectiveClass)
            .Select(m => m.TargetClass)
            .FirstOrDefaultAsync(cancellationToken);

        return (mappedClass ?? effectiveClass) switch
        {
            'F' or 'A' or 'P'        => BaggageClass.First,
            'C' or 'B' or 'J' or 'D' => BaggageClass.Business,
            _                        => BaggageClass.Economy
        };
    }
}

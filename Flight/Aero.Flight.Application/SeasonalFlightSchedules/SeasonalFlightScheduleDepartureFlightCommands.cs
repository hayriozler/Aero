using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules;

public sealed record CreateSeasonalFlightScheduleDepartureFlightCommand(SeasonalFlightScheduleDepartureFlightRequest Request);
public sealed record UpdateSeasonalFlightScheduleDepartureFlightCommand(int Id, SeasonalFlightScheduleDepartureFlightRequest Request);
public sealed record DeleteSeasonalFlightScheduleDepartureFlightCommand(int Id);

public sealed class SeasonalFlightScheduleDepartureFlightRequestValidator : AbstractValidator<SeasonalFlightScheduleDepartureFlightRequest>
{
    public SeasonalFlightScheduleDepartureFlightRequestValidator()
    {
        RuleFor(x => x.ScheduleSeasonCode).NotEmpty().MaximumLength(15);
        RuleFor(x => x.AirlineCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(10);
        RuleFor(x => x.DestinationAirportCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodBegin);
    }
}

public sealed class CreateSeasonalFlightScheduleDepartureFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<CreateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse>
{
    public async Task<SeasonalFlightScheduleDepartureFlightResponse> HandleAsync(CreateSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = Build(SeasonalFlightScheduleDepartureFlight.Create(
            command.Request.ScheduleSeasonCode, command.Request.AirlineCode, command.Request.FlightNumber,
            command.Request.Std, command.Request.PeriodBegin, command.Request.PeriodEnd, command.Request.DestinationAirportCode),
            command.Request);

        db.Add(flight);
        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse(flight);
    }

    internal static SeasonalFlightScheduleDepartureFlight Build(SeasonalFlightScheduleDepartureFlight flight, SeasonalFlightScheduleDepartureFlightRequest r)
    {
        flight.UpdateDestination(r.DestinationAirportCode);
        flight.UpdateCallSign(r.CallSign);
        flight.UpdateStatus(r.ScheduleFlightStatusCode);
        flight.UpdateCountryType(r.CountryType);
        flight.UpdateAircraftType(r.AircraftTypeCode);
        flight.UpdateOperationalDays(r.OpsDays ?? string.Empty, r.Frequency, r.IsOverMidnight);
        flight.UpdateDirection(r.DirectionCode, r.IsTransit);
        flight.UpdateComment(r.Comment);
        flight.ReplaceRoutings(r.Routings.Select(x => (x.AirportCode, x.LegNumber, x.DirectionCode, x.Sta, x.Std)));
        return flight;
    }
}

public sealed class UpdateSeasonalFlightScheduleDepartureFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse?>
{
    public async Task<SeasonalFlightScheduleDepartureFlightResponse?> HandleAsync(UpdateSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleDepartureFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return null;

        var r = command.Request;
        flight.UpdateStd(r.Std, r.Ptd);
        flight.UpdatePeriod(r.PeriodBegin, r.PeriodEnd);
        CreateSeasonalFlightScheduleDepartureFlightCommandHandler.Build(flight, r);

        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleDepartureFlightQueryHandlers.ToResponse(flight);
    }
}

public sealed class DeleteSeasonalFlightScheduleDepartureFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteSeasonalFlightScheduleDepartureFlightCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteSeasonalFlightScheduleDepartureFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleDepartureFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        db.Remove(flight);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

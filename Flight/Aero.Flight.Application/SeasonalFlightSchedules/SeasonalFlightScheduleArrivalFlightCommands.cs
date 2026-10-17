using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.SeasonalFlightSchedules;

public sealed record CreateSeasonalFlightScheduleArrivalFlightCommand(SeasonalFlightScheduleArrivalFlightRequest Request);
public sealed record UpdateSeasonalFlightScheduleArrivalFlightCommand(int Id, SeasonalFlightScheduleArrivalFlightRequest Request);
public sealed record DeleteSeasonalFlightScheduleArrivalFlightCommand(int Id);

public sealed class SeasonalFlightScheduleArrivalFlightRequestValidator : AbstractValidator<SeasonalFlightScheduleArrivalFlightRequest>
{
    public SeasonalFlightScheduleArrivalFlightRequestValidator()
    {
        RuleFor(x => x.ScheduleSeasonCode).NotEmpty().MaximumLength(15);
        RuleFor(x => x.AirlineCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(10);
        RuleFor(x => x.OriginAirportCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodBegin);
    }
}

public sealed class CreateSeasonalFlightScheduleArrivalFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<CreateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse>
{
    public async Task<SeasonalFlightScheduleArrivalFlightResponse> HandleAsync(CreateSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = Build(SeasonalFlightScheduleArrivalFlight.Create(
            command.Request.ScheduleSeasonCode, command.Request.AirlineCode, command.Request.FlightNumber,
            command.Request.Sta, command.Request.PeriodBegin, command.Request.PeriodEnd, command.Request.OriginAirportCode),
            command.Request);

        db.Add(flight);
        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse(flight);
    }

    internal static SeasonalFlightScheduleArrivalFlight Build(SeasonalFlightScheduleArrivalFlight flight, SeasonalFlightScheduleArrivalFlightRequest r)
    {
        flight.UpdateOrigin(r.OriginAirportCode);
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

public sealed class UpdateSeasonalFlightScheduleArrivalFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse?>
{
    public async Task<SeasonalFlightScheduleArrivalFlightResponse?> HandleAsync(UpdateSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleArrivalFlight>()
            .Include(f => f.Routings)
            .FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return null;

        var r = command.Request;
        flight.UpdateSta(r.Sta);
        flight.UpdatePeriod(r.PeriodBegin, r.PeriodEnd);
        CreateSeasonalFlightScheduleArrivalFlightCommandHandler.Build(flight, r);

        await db.SaveChangesAsync(ct);
        return SeasonalFlightScheduleArrivalFlightQueryHandlers.ToResponse(flight);
    }
}

public sealed class DeleteSeasonalFlightScheduleArrivalFlightCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteSeasonalFlightScheduleArrivalFlightCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteSeasonalFlightScheduleArrivalFlightCommand command, CancellationToken ct)
    {
        var flight = await db.Query<SeasonalFlightScheduleArrivalFlight>().FirstOrDefaultAsync(f => f.Id == command.Id, ct);
        if (flight is null)
            return false;

        db.Remove(flight);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

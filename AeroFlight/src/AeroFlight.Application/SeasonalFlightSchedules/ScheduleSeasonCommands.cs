using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.SeasonalFlightSchedules;
using AeroFlight.Domain.SeasonalFlightSchedules;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.SeasonalFlightSchedules;

public sealed record CreateScheduleSeasonCommand(ScheduleSeasonRequest Request);
public sealed record UpdateScheduleSeasonCommand(string Code, ScheduleSeasonRequest Request);
public sealed record DeleteScheduleSeasonCommand(string Code);

public sealed class ScheduleSeasonRequestValidator : AbstractValidator<ScheduleSeasonRequest>
{
    public ScheduleSeasonRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.BeginDate);
    }
}

public sealed class CreateScheduleSeasonCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<CreateScheduleSeasonCommand, ScheduleSeasonResponse>
{
    public async Task<ScheduleSeasonResponse> HandleAsync(CreateScheduleSeasonCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var season = ScheduleSeason.Create(r.Code, r.BeginDate, r.EndDate, r.Description);
        db.Add(season);
        await db.SaveChangesAsync(ct);
        return ScheduleSeasonQueryHandlers.ToResponse(season);
    }
}

public sealed class UpdateScheduleSeasonCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateScheduleSeasonCommand, ScheduleSeasonResponse?>
{
    public async Task<ScheduleSeasonResponse?> HandleAsync(UpdateScheduleSeasonCommand command, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(command.Code, ct);
        if (season is null)
            return null;

        var r = command.Request;
        season.UpdateDates(r.BeginDate, r.EndDate);
        if (r.Description is not null)
            season.UpdateDescription(r.Description);
        await db.SaveChangesAsync(ct);
        return ScheduleSeasonQueryHandlers.ToResponse(season);
    }
}

public sealed class DeleteScheduleSeasonCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteScheduleSeasonCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteScheduleSeasonCommand command, CancellationToken ct)
    {
        var season = await db.FindAsync<ScheduleSeason>(command.Code, ct);
        if (season is null)
            return false;

        var hasFlights = await db.Query<SeasonalFlightScheduleArrivalFlight>().AnyAsync(f => f.ScheduleSeasonCode == command.Code, ct)
            || await db.Query<SeasonalFlightScheduleDepartureFlight>().AnyAsync(f => f.ScheduleSeasonCode == command.Code, ct);
        if (hasFlights)
            throw new InvalidOperationException($"Season '{command.Code}' has schedule flights and cannot be deleted.");

        db.Remove(season);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

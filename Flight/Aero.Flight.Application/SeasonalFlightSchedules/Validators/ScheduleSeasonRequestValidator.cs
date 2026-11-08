using Aero.Flights.Contracts.SeasonalFlightSchedules;
using FluentValidation;

namespace Aero.Flights.Application.SeasonalFlightSchedules.Validators;

public sealed class ScheduleSeasonRequestValidator : AbstractValidator<ScheduleSeasonRequest>
{
    public ScheduleSeasonRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.BeginDate);
    }
}

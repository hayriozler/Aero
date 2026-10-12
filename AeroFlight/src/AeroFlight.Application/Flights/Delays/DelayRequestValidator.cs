using AeroFlight.Contracts.Flights;
using FluentValidation;

namespace AeroFlight.Application.Flights.Delays;

public sealed class DelayRequestValidator : AbstractValidator<DelayRequest>
{
    public DelayRequestValidator()
    {
        RuleFor(x => x.DelayCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.DelayMinutes).GreaterThanOrEqualTo(0).When(x => x.DelayMinutes is not null);
        RuleFor(x => x.Comment).MaximumLength(500);
    }
}

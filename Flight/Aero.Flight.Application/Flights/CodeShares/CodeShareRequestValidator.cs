using Aero.Flights.Contracts.Flights;
using FluentValidation;

namespace Aero.Flights.Application.Flights.CodeShares;

public sealed class CodeShareRequestValidator : AbstractValidator<CodeShareRequest>
{
    public CodeShareRequestValidator()
    {
        RuleFor(x => x.AirlineCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.FlightNumber).NotEmpty().MaximumLength(FlightNumberFormat.MaxLength)
            .Must(FlightNumberFormat.IsValid)
            .WithMessage("Flight number must be a code (max 3 chars) followed by a number (max 5 digits), e.g. \"TK 2563\" or \"SXS45862\".");
    }
}

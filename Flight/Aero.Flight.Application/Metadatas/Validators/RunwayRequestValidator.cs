using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class RunwayRequestValidator : AbstractValidator<RunwayRequest>
{
    public RunwayRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Name).MaximumLength(100);
    }
}
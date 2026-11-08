using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AirlineLogoRenameRequestValidator : AbstractValidator<AirlineLogoRenameRequest>
{
    public AirlineLogoRenameRequestValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
}

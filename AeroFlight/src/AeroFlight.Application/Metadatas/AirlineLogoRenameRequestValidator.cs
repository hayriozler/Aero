using AeroFlight.Contracts.Metadatas;
using FluentValidation;

namespace AeroFlight.Application.Metadatas;

public sealed class AirlineLogoRenameRequestValidator : AbstractValidator<AirlineLogoRenameRequest>
{
    public AirlineLogoRenameRequestValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
}

using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;

namespace AeroFlight.Application.Metadatas;

public sealed class AirlineContactRequestValidator : AbstractValidator<AirlineContactRequest>
{
    public AirlineContactRequestValidator()
    {
        RuleFor(x => x.ContactType)
            .Must(t => Enum.TryParse<AirlineContactType>(t, true, out _))
            .WithMessage($"ContactType must be one of: {string.Join(", ", Enum.GetNames<AirlineContactType>())}");
        RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
    }
}

using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;

namespace AeroFlight.Application.Metadatas;

public sealed class CompanyContactRequestValidator : AbstractValidator<CompanyContactRequest>
{
    public CompanyContactRequestValidator()
    {
        RuleFor(x => x.ContactType)
            .Must(t => Enum.TryParse<CompanyContactType>(t, true, out _))
            .WithMessage($"ContactType must be one of: {string.Join(", ", Enum.GetNames<CompanyContactType>())}");
        RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
    }
}

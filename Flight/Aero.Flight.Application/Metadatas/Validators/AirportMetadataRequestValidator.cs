using Aero.Flights.Contracts.Metadatas;
using FluentValidation;

namespace Aero.Flights.Application.Metadatas.Validators;

public sealed class AirportMetadataRequestValidator : AbstractValidator<AirportRequest>
{
    public AirportMetadataRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.IataCode).MaximumLength(3);
        RuleFor(x => x.IcaoCode).MaximumLength(4);
        RuleFor(x => x.Name).MaximumLength(50);
        RuleFor(x => x.DisplayName).MaximumLength(100);
        RuleFor(x => x.CountryCode).MaximumLength(3);
        RuleFor(x => x.TimezoneCode).MaximumLength(50);
    }
}

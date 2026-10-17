using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class CountryRequestValidator : AbstractValidator<CountryRequest>
{
    public CountryRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CountryTypeCode).MaximumLength(10);
    }
}

public sealed class CountryMetadataMap : IMetadataMap<Country, CountryRequest, CountryResponse>
{
    public string GetCode(Country entity) => entity.Id;

    public Country Create(CountryRequest r) => Country.Create(r.Code, r.Name, r.CountryTypeCode);

    public void Apply(Country e, CountryRequest r) => e.Update(r.Name, r.CountryTypeCode);

    public CountryResponse ToResponse(Country e) => new(
        e.Id, e.Name, e.CountryTypeCode);

    public Expression<Func<Country, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Country, CountryResponse>> Projection { get; } = e => new CountryResponse(
        e.Id, e.Name, e.CountryTypeCode);
}

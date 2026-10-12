using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

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

public sealed class AirportMetadataMap : IMetadataMap<Airport, AirportRequest, AirportResponse>
{
    public string GetCode(Airport entity) => entity.Id;

    public Airport Create(AirportRequest r) => Airport.Create(r.Code, r.IataCode, r.Name, r.DisplayName);

    public void Apply(Airport e, AirportRequest r) => e.Update(
        r.IataCode, r.IcaoCode, r.Name, r.DisplayName, r.CountryCode, r.TimezoneCode);

    public AirportResponse ToResponse(Airport e) => new(
        e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);

    public Expression<Func<Airport, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<Airport, AirportResponse>> Projection { get; } = e => new AirportResponse(
        e.Id, e.IataCode, e.IcaoCode, e.Name, e.DisplayName, e.CountryCode, e.TimezoneCode);
}

using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class AirlineRequestValidator : AbstractValidator<AirlineRequest>
{
    public AirlineRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.IataCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.IcaoCode).MaximumLength(4);
        RuleFor(x => x.Callsign).MaximumLength(50);
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.DisplayName).MaximumLength(200);
        RuleFor(x => x.ParentAirlineCode).MaximumLength(10);
        RuleFor(x => x.CountryCode).MaximumLength(3);
    }
}

public sealed class AirlineMetadataMap : IMetadataMap<Airline, AirlineRequest, AirlineResponse>
{
    public string GetCode(Airline entity) => entity.Id;

    public Airline Create(AirlineRequest r) => Airline.Create(r.Code, r.IataCode, r.Name, r.DisplayName);

    public void Apply(Airline e, AirlineRequest r) => e.Update(
        r.IcaoCode, r.Callsign, r.Name, r.DisplayName,
        r.ParentAirlineCode, r.CountryCode);

    public AirlineResponse ToResponse(Airline e) => new(
        e.Id, e.IataCode, e.IcaoCode, e.FlightNumberCode, e.Callsign, e.Name, e.DisplayName,
        e.ParentAirlineCode, e.CountryCode);

    public Expression<Func<Airline, string>> CodeSelector { get; } = e => e.Id;

    // Same shape as ToResponse, but as an expression so MetadataListQueryHandler can hand it to EF
    // and get a SELECT with only these columns instead of loading the full Airline row.
    public Expression<Func<Airline, AirlineResponse>> Projection { get; } = e => new AirlineResponse(
        e.Id, e.IataCode, e.IcaoCode, e.FlightNumberCode, e.Callsign, e.Name, e.DisplayName,
        e.ParentAirlineCode, e.CountryCode);
}

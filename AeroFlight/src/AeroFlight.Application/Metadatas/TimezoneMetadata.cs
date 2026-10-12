using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class TimezoneReader : IMetadataReader<Timezone, TimezoneResponse>
{
    public string GetCode(Timezone entity) => entity.Code;
    public TimezoneResponse ToResponse(Timezone entity) => new(entity.Code, entity.Description);

    public Expression<Func<Timezone, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<Timezone, TimezoneResponse>> Projection { get; } =
        e => new TimezoneResponse(e.Code, e.Description);
}

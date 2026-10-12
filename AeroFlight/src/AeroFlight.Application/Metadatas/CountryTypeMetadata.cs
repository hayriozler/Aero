using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using System.Linq.Expressions;

namespace AeroFlight.Application.Metadatas;

public sealed class CountryTypeReader : IMetadataReader<CountryType, CountryTypeResponse>
{
    public string GetCode(CountryType entity) => entity.Code;
    public CountryTypeResponse ToResponse(CountryType entity) => new(entity.Code, entity.Description);

    public Expression<Func<CountryType, string>> CodeSelector { get; } = e => e.Code;
    public Expression<Func<CountryType, CountryTypeResponse>> Projection { get; } =
        e => new CountryTypeResponse(e.Code, e.Description);
}

using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;

public class ArrivalBaggageStatusEnumConverter : ValueConverter<ArrivalBaggageStatus, string>
{
    public ArrivalBaggageStatusEnumConverter() : base(
          v => ToChar(v).ToString(),
          v => FromChar(v[0]))
    { }

    private static char ToChar(ArrivalBaggageStatus status) => status switch
    {
        ArrivalBaggageStatus.Arrived => 'A',
        ArrivalBaggageStatus.Waiting => 'W',
        ArrivalBaggageStatus.OnBelt => 'B',
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static ArrivalBaggageStatus FromChar(char c) => c switch
    {
        'A' => ArrivalBaggageStatus.Arrived,
        'W' => ArrivalBaggageStatus.Waiting,
        'B' => ArrivalBaggageStatus.OnBelt,
        _ => throw new Exception($"Invalid status: {c}")
    };
}

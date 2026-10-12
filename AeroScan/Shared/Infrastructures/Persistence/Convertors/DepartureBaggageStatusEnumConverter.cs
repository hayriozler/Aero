using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;


public class DepartureBaggageStatusEnumConverter : ValueConverter<DepartureBaggageStatus, string>
{
    public DepartureBaggageStatusEnumConverter() : base(
          v => ToChar(v).ToString(),
          v => FromChar(v[0]))
    { }

    private static char ToChar(DepartureBaggageStatus status) => status switch
    {
        DepartureBaggageStatus.Unknown => 'K',
        DepartureBaggageStatus.Waiting => 'W',
        DepartureBaggageStatus.Sorted => 'S',
        DepartureBaggageStatus.Loaded => 'L',
        DepartureBaggageStatus.ForceLoaded => 'F',
        DepartureBaggageStatus.ToBeOffload => 'O',
        DepartureBaggageStatus.Deleted => 'D',
        DepartureBaggageStatus.UnLoaded => 'U',
        DepartureBaggageStatus.Transferred => 'T',
        DepartureBaggageStatus.Cancelled => 'C',
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static DepartureBaggageStatus FromChar(char c) => c switch
    {
        'K' => DepartureBaggageStatus.Unknown,
        'W' => DepartureBaggageStatus.Waiting,
        'S' => DepartureBaggageStatus.Sorted,
        'L' => DepartureBaggageStatus.Loaded,
        'F' => DepartureBaggageStatus.ForceLoaded,
        'O' => DepartureBaggageStatus.ToBeOffload,
        'D' => DepartureBaggageStatus.Deleted,
        'U' => DepartureBaggageStatus.UnLoaded,
        'T' => DepartureBaggageStatus.Transferred,
        'C' => DepartureBaggageStatus.Cancelled,
        _ => throw new Exception($"Invalid status: {c}")
    };
}


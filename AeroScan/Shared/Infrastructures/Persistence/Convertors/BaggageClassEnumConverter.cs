using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;

public class BaggageClassEnumConverter : ValueConverter<BaggageClass, string>
{
  public BaggageClassEnumConverter() : base(
        v => ToChar(v).ToString(),
        v => FromChar(v[0]))
    { }

    private static char ToChar(BaggageClass status) => status switch
    {
        BaggageClass.Economy=> 'L',
        BaggageClass.First=> 'F',
        BaggageClass.Business=> 'C',
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static BaggageClass FromChar(char c) => c switch
    {
        'L' => BaggageClass.Economy,
        'F' => BaggageClass.First,
        'C' => BaggageClass.Business,
        _ => throw new Exception($"Invalid status: {c}")
    };
}

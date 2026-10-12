using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;

public class PassengerStatusEnumConverter : ValueConverter<PassengerStatus, string>
{
  public PassengerStatusEnumConverter() : base(
        v => ToChar(v).ToString(),
        v => FromChar(v[0]))
    { }

    private static char ToChar(PassengerStatus status) => status switch
    {
        PassengerStatus.None => 'N',
        PassengerStatus.Boarded=> 'B',
        PassengerStatus.CheckIn=> 'C',
        PassengerStatus.NoShow=> 'S',
        PassengerStatus.Deboarded=> 'D',
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static PassengerStatus FromChar(char c) => c switch
    {
        'N' => PassengerStatus.None,
        'B' => PassengerStatus.Boarded,
        'C' => PassengerStatus.CheckIn,
        'S' => PassengerStatus.NoShow,
        'D' => PassengerStatus.Deboarded,
        _ => throw new Exception($"Invalid status: {c}")
    };
}

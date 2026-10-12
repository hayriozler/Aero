using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;
public class DepartureFlightStatusEnumConverter : ValueConverter<DepartureFlightStatus, string>
{
    public DepartureFlightStatusEnumConverter() : base(
        v => ToChar(v).ToString(),
        v => FromChar(v[0]))
    { }

    private static char ToChar(DepartureFlightStatus status) => status switch
    {
        DepartureFlightStatus.Scheduled => 'S',
        DepartureFlightStatus.Estimated => 'E',
        DepartureFlightStatus.Delayed => 'Y',
        DepartureFlightStatus.CheckInOpen => 'O',
        DepartureFlightStatus.Boarding => 'B',
        DepartureFlightStatus.FinalCall => 'F',
        DepartureFlightStatus.GateClosed => 'G',
        DepartureFlightStatus.Departed => 'D',
        DepartureFlightStatus.Cancelled => 'C',
        _ => throw new Exception($"Invalid status: {status}")
    };

    private static DepartureFlightStatus FromChar(char c) => c switch
    {
        'S' => DepartureFlightStatus.Scheduled,
        'E' => DepartureFlightStatus.Estimated,
        'Y' => DepartureFlightStatus.Delayed,
        'O' => DepartureFlightStatus.CheckInOpen,
        'B' => DepartureFlightStatus.Boarding,
        'F' => DepartureFlightStatus.FinalCall,
        'G' => DepartureFlightStatus.GateClosed,
        'D' => DepartureFlightStatus.Departed,
        'C' => DepartureFlightStatus.Cancelled,
        _ => throw new Exception($"Invalid status: {c}")
    };
}

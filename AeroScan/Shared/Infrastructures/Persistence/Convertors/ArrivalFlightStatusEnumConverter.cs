using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Convertors;

public class ArrivalFlightStatusEnumConverter : ValueConverter<ArrivalFlightStatus, string>
{
    public ArrivalFlightStatusEnumConverter() : base(
        v => ToChar(v).ToString(),
        v => FromChar(v[0]))
    { }

    private static char ToChar(ArrivalFlightStatus status) => status switch
    {
        ArrivalFlightStatus.Scheduled => 'S',
        ArrivalFlightStatus.Estimated => 'E',
        ArrivalFlightStatus.Landed => 'Y',
        ArrivalFlightStatus.Arrived =>'A',
        ArrivalFlightStatus.Delayed => 'O',
        ArrivalFlightStatus.Cancelled => 'C',
        ArrivalFlightStatus.Diverted => 'V',
        _ => throw new Exception($"Invalid status: {status}")
    };

    /*
Scheduled = 0,
Estimated = 1,
Landed = 2,
Arrived = 3,
Delayed = 4 ,
Cancelled = 5,
Diverted = 6*/
    private static ArrivalFlightStatus FromChar(char c) => c switch
    {
        'S' => ArrivalFlightStatus.Scheduled,
        'E' => ArrivalFlightStatus.Estimated,
        'Y' => ArrivalFlightStatus.Landed,
        'A' => ArrivalFlightStatus.Arrived,
        'O' => ArrivalFlightStatus.Delayed,
        'C' => ArrivalFlightStatus.Cancelled,
        'V' => ArrivalFlightStatus.Diverted,
        _ => throw new Exception($"Invalid status: {c}")
    };
}
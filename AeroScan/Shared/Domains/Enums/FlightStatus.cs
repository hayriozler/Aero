namespace Domain.Enums;

public enum DepartureFlightStatus
{
    Scheduled = 0,
    Estimated = 1,
    Delayed = 3,
    CheckInOpen = 4,
    Boarding = 5,
    FinalCall = 6,
    GateClosed = 7,
    Departed = 8,    
    Cancelled = 9    
}

public enum ArrivalFlightStatus
{
    Scheduled = 0,
    Estimated = 1,
    Landed = 2,
    Arrived = 3,
    Delayed = 4 ,
    Cancelled = 5,
    Diverted = 6
}

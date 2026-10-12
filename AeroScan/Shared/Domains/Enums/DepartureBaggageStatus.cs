namespace Domain.Enums;

public enum DepartureBaggageStatus
{
    Unknown, // K,
    Waiting, // = 'W',
    Sorted, // = 'S',
    Loaded, // = 'L',
    ForceLoaded, // = 'F',
    ToBeOffload, // = 'O',
    Deleted, // = 'D',
    UnLoaded, // = 'U',
    Transferred, // = 'T',
    Cancelled, // = 'C'
}

namespace Aero.Fids.Infrastructure.Boards;

public static class DisplayClientTypes
{
    public const string DepartureFlightList = "DepartureFlightList";
    public const string ArrivalFlightList = "ArrivalFlightList";
    public const string FullFlightList = "FullFlightList";
    public const string Counter = "Counter";
    public const string Gate = "Gate";
    public const string DepartureBelt = "DepartureBelt";
    public const string ArrivalBelt = "ArrivalBelt";
    public const string Spot = "Spot";

    public static readonly IReadOnlyList<string> All =
    [
        DepartureFlightList, ArrivalFlightList, FullFlightList,
        Counter, Gate, DepartureBelt, ArrivalBelt, Spot,
    ];

    // The single-physical-instance types (as opposed to the 3 whole-airport flight-list types) —
    // distinct from Aero.Fids.Infrastructure.Flights.ResourceKinds, which has one shared "Belt" kind
    // for both ArrivalBelt and DepartureBelt.
    public static readonly IReadOnlyList<string> ResourceTypes =
    [
        Counter, Gate, DepartureBelt, ArrivalBelt, Spot,
    ];
}

using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Ops.Passengers;

public abstract class OpsPassenger : FlightResourceBase
{
    public string? PassengerType { get; private set; }
    public int Count { get; private set; }

    protected OpsPassenger() { }

    protected OpsPassenger(long flightId, string? passengerType, int count) : base(flightId)
    {
        PassengerType = passengerType;
        Count = count;
    }

    public void Update(int count) => Count = count;
}

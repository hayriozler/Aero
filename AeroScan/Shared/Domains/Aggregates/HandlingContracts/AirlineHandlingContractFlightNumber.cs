using Domain.Common;

namespace Domain.Aggregates.HandlingContracts;

public sealed class AirlineHandlingContractFlightNumber : Entity<int>
{
    public int ContractId { get; private set; }

    public string FlightNumber { get; private set; } = string.Empty;

    private AirlineHandlingContractFlightNumber() { }

    internal static AirlineHandlingContractFlightNumber Create(int contractId, string flightNumber) =>
        new() { ContractId = contractId, FlightNumber = flightNumber };
}

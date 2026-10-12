using Domain.Common;

namespace Domain.Aggregates.Flights;

public sealed class Container : Entity<int>
{
    public int FlightId { get; private set; }
    public string ContainerCode { get; private set; } = string.Empty;
    public string ContainerTypeCode { get; private set; } = string.Empty;
    public string ContainerStatusCode { get; private set; } = string.Empty;
    public string ContainerClassCode { get; private set; } = string.Empty;
    public string ContainerDestination { get; private set; } = string.Empty;

    private Container() { }

    public static Container Create(
        int flightId,
        string containerCode,
        string containerTypeCode,
        string containerStatusCode,
        string containerClassCode,
        string containerDestination) => new()
        {
            FlightId             = flightId,
            ContainerCode        = containerCode.ToUpperInvariant().Trim(),
            ContainerTypeCode    = containerTypeCode.ToUpperInvariant().Trim(),
            ContainerStatusCode  = containerStatusCode.ToUpperInvariant().Trim(),
            ContainerClassCode   = containerClassCode.ToUpperInvariant().Trim(),
            ContainerDestination = containerDestination.ToUpperInvariant().Trim()
        };

    public void Update(
        string containerCode,
        string containerTypeCode,
        string containerStatusCode,
        string containerClassCode,
        string containerDestination)
    {
        ContainerCode        = containerCode.ToUpperInvariant().Trim();
        ContainerTypeCode    = containerTypeCode.ToUpperInvariant().Trim();
        ContainerStatusCode  = containerStatusCode.ToUpperInvariant().Trim();
        ContainerClassCode   = containerClassCode.ToUpperInvariant().Trim();
        ContainerDestination = containerDestination.ToUpperInvariant().Trim();
    }
}
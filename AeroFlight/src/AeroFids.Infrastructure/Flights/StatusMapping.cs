namespace AeroFids.Infrastructure.Flights;

public static class StatusMappingDimensions
{
    public const string Flight = "Flight";
    public const string Gate = ResourceKinds.Gate;
    public const string Belt = ResourceKinds.Belt;
    public const string Counter = ResourceKinds.Counter;
    public const string Spot = ResourceKinds.Spot;

    public static readonly IReadOnlyList<string> All = [Flight, Gate, Belt, Counter, Spot];
}

public sealed class StatusMapping
{
    public long Id { get; private set; }
    public string Dimension { get; private set; } = string.Empty;

    public string? FlightType { get; private set; }
    public string ExternalValue { get; private set; } = string.Empty;
    public string InternalValue { get; private set; } = string.Empty;

    private StatusMapping() { }

    public static StatusMapping Create(string dimension, string? flightType, string externalValue, string internalValue) => new()
    {
        Dimension = dimension,
        FlightType = flightType,
        ExternalValue = externalValue,
        InternalValue = internalValue,
    };

    public void Update(string internalValue) => InternalValue = internalValue;
}

using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class Aircraft : AuditBase<string>
{
    public string AircraftTypeCode { get; private set; } = string.Empty;
    public string? AirlineCode { get; private set; }
    public string? Owner { get; private set; }
    public string? SizeCategory { get; private set; }
    public string? NoiseCategory { get; private set; }
    public decimal? Weight { get; private set; }
    public int? SeatCapacity { get; private set; }
    private Aircraft() { }

    private Aircraft(string code, string aircraftTypeCode)
    {
        SetId(code);
        AircraftTypeCode = aircraftTypeCode;
    }

    public static Aircraft Create(string code, string aircraftTypeCode) => new(code, aircraftTypeCode);

    public void Update(string? airlineCode, string? owner,
        string? sizeCategory, string? noiseCategory, decimal? billWeight, int? seatCapacity)
    {
        AirlineCode = airlineCode;
        Owner = owner;
        SizeCategory = sizeCategory;
        NoiseCategory = noiseCategory;
        Weight = billWeight;
        SeatCapacity = seatCapacity;
    }
}

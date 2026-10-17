using Aero.Flights.Domain.Common;
namespace Aero.Flights.Domain.Metadatas;

public sealed class AircraftType : AuditBase<string>
{
    public string? IataCode { get; private set; }
    public string? IataSubCode { get; private set; }
    public string? IcaoCode { get; private set; }
    public string? SizeCategory { get; private set; }
    public string? NoiseCategory { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public decimal? Mtow { get; private set; }
    public decimal? LandingWeight { get; private set; }
    public int? SeatCapacity { get; private set; }
    public bool IsBridgeRequired { get; private set; }

    private AircraftType() { }

    private AircraftType(string code) => SetId(code);

    public static AircraftType Create(string code) => new(code);

    public void Update(
        string? iataCode,
        string? iataSubCode,
        string? icaoCode, string? sizeCategory, string? noiseCategory, string? manufacturer, string? model, decimal? mtow, decimal? landingWeight,
        int? seatCapacity, bool isBridgeRequired)
    {
        IataCode = iataCode;
        IataSubCode = iataSubCode;
        IcaoCode = icaoCode;
        SizeCategory = sizeCategory;
        NoiseCategory = noiseCategory;
        Manufacturer = manufacturer;
        Model = model;
        Mtow = mtow;
        LandingWeight = landingWeight;
        SeatCapacity = seatCapacity;
        IsBridgeRequired = isBridgeRequired;
    }
}

using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Metadatas;

public sealed class ParkPosition : AuditBase<string>
{
    public string? TypeCode { get; private set; }
    public string? Description { get; private set; }
    public int? Capacity { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public decimal? Angle { get; private set; }

    private ParkPosition() { }

    private ParkPosition(string code) => SetId(code);

    public static ParkPosition Create(string code) => new(code);

    public void Update(string? typeCode, string? description,
        int? capacity,
        decimal? latitude, decimal? longitude, decimal? angle)
    {
        TypeCode = typeCode;
        Description = description;
        Capacity = capacity;
        Latitude = latitude;
        Longitude = longitude;
        Angle = angle;
    }
}

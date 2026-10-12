namespace AeroFlight.Domain.Metadatas;

public sealed class AirportImage
{
    public int Id { get; private set; }
    public string AirportCode { get; private set; } = string.Empty;
    public byte[] ImageData { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    private AirportImage() { }

    private AirportImage(string airportCode, byte[] imageData, string contentType, int displayOrder)
    {
        AirportCode = airportCode;
        ImageData = imageData;
        ContentType = contentType;
        DisplayOrder = displayOrder;
    }

    public static AirportImage Create(string airportCode, byte[] imageData, string contentType, int displayOrder)
        => new(airportCode, imageData, contentType, displayOrder);
}

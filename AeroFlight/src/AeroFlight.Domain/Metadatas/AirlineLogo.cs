namespace AeroFlight.Domain.Metadatas;

public sealed class AirlineLogo
{
    public int Id { get; private set; }
    public string AirlineCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public byte[] ImageData { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;

    private AirlineLogo() { }

    private AirlineLogo(string airlineCode, string name, byte[] imageData, string contentType)
    {
        AirlineCode = airlineCode;
        Name = name;
        ImageData = imageData;
        ContentType = contentType;
    }

    public static AirlineLogo Create(string airlineCode, string name, byte[] imageData, string contentType)
        => new(airlineCode, name, imageData, contentType);

    public void Rename(string name) => Name = name;
}

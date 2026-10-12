namespace AeroFlight.Domain.Metadatas;

public class Region 
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Region() { }

}

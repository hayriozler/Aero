namespace AeroFlight.Domain.Metadatas;

public sealed class DeBoardingStatus
{
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private DeBoardingStatus() { }   
}

namespace AeroFids.Infrastructure.Signage;

public sealed class SignageImage
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public byte[] ImageData { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    private SignageImage() { }

    public static SignageImage Create(string name, byte[] imageData, string contentType, DateOnly? startDate, DateOnly? endDate, int displayOrder) => new()
    {
        Name = name,
        ImageData = imageData,
        ContentType = contentType,
        StartDate = startDate,
        EndDate = endDate,
        DisplayOrder = displayOrder,
        IsActive = true,
    };

    public void Update(string name, DateOnly? startDate, DateOnly? endDate, int displayOrder, bool isActive)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        DisplayOrder = displayOrder;
        IsActive = isActive;
    }
}

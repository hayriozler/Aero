namespace Aero.Fids.Application.Signages;

public sealed record SignageImageResponse(
    long Id, string Name, DateOnly? StartDate, DateOnly? EndDate, int DisplayOrder, bool IsActive, string ContentType, int ImageSizeBytes);

using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Metadatas;

public sealed record ListAirportImagesQuery(string AirportCode);
public sealed record GetAirportImageQuery(string AirportCode, int Id);
public sealed record AirportImageData(byte[] Data, string ContentType);
public sealed record AddAirportImageCommand(string AirportCode, byte[] Data, string ContentType, int? DisplayOrder);
public sealed record DeleteAirportImageCommand(string AirportCode, int Id);

public sealed class ListAirportImagesQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListAirportImagesQuery, IReadOnlyList<AirportImageResponse>>
{
    public async Task<IReadOnlyList<AirportImageResponse>> HandleAsync(ListAirportImagesQuery query, CancellationToken ct) =>
        await db.Query<AirportImage>()
            .Where(i => i.AirportCode == query.AirportCode)
            .OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id)
            .Select(i => new AirportImageResponse(i.Id, i.AirportCode, i.ContentType, i.ImageData.Length, i.DisplayOrder))
            .ToListAsync(ct);
}

public sealed class GetAirportImageQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetAirportImageQuery, AirportImageData?>
{
    public async Task<AirportImageData?> HandleAsync(GetAirportImageQuery query, CancellationToken ct)
    {
        var image = await db.Query<AirportImage>().FirstOrDefaultAsync(i => i.Id == query.Id && i.AirportCode == query.AirportCode, ct);
        return image is null ? null : new AirportImageData(image.ImageData, image.ContentType);
    }
}

public sealed class AddAirportImageCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<AddAirportImageCommand, AirportImageResponse>
{
    public async Task<AirportImageResponse> HandleAsync(AddAirportImageCommand command, CancellationToken ct)
    {
        var displayOrder = command.DisplayOrder
            ?? await db.Query<AirportImage>().Where(i => i.AirportCode == command.AirportCode).CountAsync(ct);

        var image = AirportImage.Create(command.AirportCode, command.Data, command.ContentType, displayOrder);
        db.Add(image);
        await db.SaveChangesAsync(ct);
        return new AirportImageResponse(image.Id, image.AirportCode, image.ContentType, image.ImageData.Length, image.DisplayOrder);
    }
}

public sealed class DeleteAirportImageCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteAirportImageCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteAirportImageCommand command, CancellationToken ct)
    {
        var image = await db.Query<AirportImage>().FirstOrDefaultAsync(i => i.Id == command.Id && i.AirportCode == command.AirportCode, ct);
        if (image is null) return false;

        db.Remove(image);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

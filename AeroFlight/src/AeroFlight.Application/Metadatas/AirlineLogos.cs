using AeroFlight.Application.Common;
using AeroFlight.Application.Interfaces;
using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace AeroFlight.Application.Metadatas;

public sealed record ListAirlineLogosQuery(string AirlineCode);
public sealed record GetAirlineLogoImageQuery(string AirlineCode, int Id);
public sealed record AirlineLogoImage(byte[] Data, string ContentType);
public sealed record AddAirlineLogoCommand(string AirlineCode, string Name, byte[] Data, string ContentType);
public sealed record UpdateAirlineLogoNameCommand(string AirlineCode, int Id, string Name);
public sealed record DeleteAirlineLogoCommand(string AirlineCode, int Id);

public sealed class ListAirlineLogosQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListAirlineLogosQuery, IReadOnlyList<AirlineLogoResponse>>
{
    public async Task<IReadOnlyList<AirlineLogoResponse>> HandleAsync(ListAirlineLogosQuery query, CancellationToken ct) =>
        await db.Query<AirlineLogo>()
            .Where(l => l.AirlineCode == query.AirlineCode)
            .OrderBy(l => l.Id)
            .Select(l => new AirlineLogoResponse(l.Id, l.AirlineCode, l.Name, l.ContentType, l.ImageData.Length))
            .ToListAsync(ct);
}

public sealed class GetAirlineLogoImageQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<GetAirlineLogoImageQuery, AirlineLogoImage?>
{
    public async Task<AirlineLogoImage?> HandleAsync(GetAirlineLogoImageQuery query, CancellationToken ct)
    {
        var logo = await db.Query<AirlineLogo>()
            .FirstOrDefaultAsync(l => l.AirlineCode == query.AirlineCode && l.Id == query.Id, ct);
        return logo is null ? null : new AirlineLogoImage(logo.ImageData, logo.ContentType);
    }
}

public sealed class AddAirlineLogoCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<AddAirlineLogoCommand, AirlineLogoResponse>
{
    public async Task<AirlineLogoResponse> HandleAsync(AddAirlineLogoCommand command, CancellationToken ct)
    {
        var logo = AirlineLogo.Create(command.AirlineCode, command.Name, command.Data, command.ContentType);
        db.Add(logo);
        await db.SaveChangesAsync(ct);
        return new AirlineLogoResponse(logo.Id, logo.AirlineCode, logo.Name, logo.ContentType, logo.ImageData.Length);
    }
}

public sealed class UpdateAirlineLogoNameCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateAirlineLogoNameCommand, AirlineLogoResponse?>
{
    public async Task<AirlineLogoResponse?> HandleAsync(UpdateAirlineLogoNameCommand command, CancellationToken ct)
    {
        var logo = await db.Query<AirlineLogo>()
            .FirstOrDefaultAsync(l => l.AirlineCode == command.AirlineCode && l.Id == command.Id, ct);
        if (logo is null) return null;

        logo.Rename(command.Name);
        await db.SaveChangesAsync(ct);
        return new AirlineLogoResponse(logo.Id, logo.AirlineCode, logo.Name, logo.ContentType, logo.ImageData.Length);
    }
}

public sealed class DeleteAirlineLogoCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteAirlineLogoCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteAirlineLogoCommand command, CancellationToken ct)
    {
        var logo = await db.Query<AirlineLogo>()
            .FirstOrDefaultAsync(l => l.AirlineCode == command.AirlineCode && l.Id == command.Id, ct);
        if (logo is null) return false;

        db.Remove(logo);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

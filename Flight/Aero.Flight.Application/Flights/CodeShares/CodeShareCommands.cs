using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Flights.CodeShares;

public sealed record ListCodeSharesQuery<TCodeShare>(long FlightId) where TCodeShare : OpsCodeShare;
public sealed record AddCodeShareCommand<TCodeShare>(long FlightId, CodeShareRequest Request) where TCodeShare : OpsCodeShare;
public sealed record UpdateCodeShareCommand<TCodeShare>(long FlightId, long CodeShareId, CodeShareRequest Request) where TCodeShare : OpsCodeShare;
public sealed record DeleteCodeShareCommand<TCodeShare>(long FlightId, long CodeShareId) where TCodeShare : OpsCodeShare;

public sealed class ListCodeSharesQueryHandler<TCodeShare>(IAeroFlightDbContext db)
    : IQueryHandler<ListCodeSharesQuery<TCodeShare>, IReadOnlyList<CodeShareResponse>>
    where TCodeShare : OpsCodeShare
{
    public async Task<IReadOnlyList<CodeShareResponse>> HandleAsync(ListCodeSharesQuery<TCodeShare> query, CancellationToken ct)
    {
        var items = await db.Query<TCodeShare>()
            .Where(c => c.FlightId == query.FlightId)
            .OrderBy(c => c.Id)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static CodeShareResponse ToResponse(TCodeShare c) => new(c.Id, c.FlightId, c.AirlineCode, c.FlightNumber);
}

public sealed class AddCodeShareCommandHandler<TCodeShare>(IAeroFlightDbContext db, ICodeShareFactory<TCodeShare> factory)
    : ICommandHandler<AddCodeShareCommand<TCodeShare>, CodeShareResponse>
    where TCodeShare : OpsCodeShare
{
    public async Task<CodeShareResponse> HandleAsync(AddCodeShareCommand<TCodeShare> command, CancellationToken ct)
    {
        var r = command.Request;
        var codeShare = factory.Create(command.FlightId, r.AirlineCode, r.FlightNumber);
        db.Add(codeShare);
        await db.SaveChangesAsync(ct);
        return ListCodeSharesQueryHandler<TCodeShare>.ToResponse(codeShare);
    }
}

public sealed class UpdateCodeShareCommandHandler<TCodeShare>(IAeroFlightDbContext db)
    : ICommandHandler<UpdateCodeShareCommand<TCodeShare>, CodeShareResponse?>
    where TCodeShare : OpsCodeShare
{
    public async Task<CodeShareResponse?> HandleAsync(UpdateCodeShareCommand<TCodeShare> command, CancellationToken ct)
    {
        var codeShare = await db.Query<TCodeShare>()
            .FirstOrDefaultAsync(c => c.Id == command.CodeShareId && c.FlightId == command.FlightId, ct);
        if (codeShare is null)
            return null;

        var r = command.Request;
        codeShare.UpdateDesignator(r.AirlineCode, r.FlightNumber);
        await db.SaveChangesAsync(ct);
        return ListCodeSharesQueryHandler<TCodeShare>.ToResponse(codeShare);
    }
}

public sealed class DeleteCodeShareCommandHandler<TCodeShare>(IAeroFlightDbContext db)
    : ICommandHandler<DeleteCodeShareCommand<TCodeShare>, bool>
    where TCodeShare : OpsCodeShare
{
    public async Task<bool> HandleAsync(DeleteCodeShareCommand<TCodeShare> command, CancellationToken ct)
    {
        var codeShare = await db.Query<TCodeShare>()
            .FirstOrDefaultAsync(c => c.Id == command.CodeShareId && c.FlightId == command.FlightId, ct);
        if (codeShare is null)
            return false;

        db.Remove(codeShare);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

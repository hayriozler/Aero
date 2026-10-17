using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
namespace Aero.Flights.Application.Metadatas;

public sealed record ListAirlineContactsQuery(string AirlineCode);
public sealed record AddAirlineContactCommand(string AirlineCode, AirlineContactRequest Request);
public sealed record UpdateAirlineContactCommand(string AirlineCode, int Id, AirlineContactRequest Request);
public sealed record DeleteAirlineContactCommand(string AirlineCode, int Id);

public sealed class ListAirlineContactsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListAirlineContactsQuery, IReadOnlyList<AirlineContactResponse>>
{
    public async Task<IReadOnlyList<AirlineContactResponse>> HandleAsync(ListAirlineContactsQuery query, CancellationToken ct)
    {
        var items = await db.Query<AirlineContact>()
            .Where(c => c.AirlineCode == query.AirlineCode)
            .OrderBy(c => c.Id)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static AirlineContactResponse ToResponse(AirlineContact c) =>
        new(c.Id, c.AirlineCode, c.ContactType.ToString(), c.Value);
}

public sealed class AddAirlineContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<AddAirlineContactCommand, AirlineContactResponse>
{
    public async Task<AirlineContactResponse> HandleAsync(AddAirlineContactCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var contact = AirlineContact.Create(command.AirlineCode, Enum.Parse<AirlineContactType>(r.ContactType, true), r.Value);
        db.Add(contact);
        await db.SaveChangesAsync(ct);
        return ListAirlineContactsQueryHandler.ToResponse(contact);
    }
}

public sealed class UpdateAirlineContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateAirlineContactCommand, AirlineContactResponse?>
{
    public async Task<AirlineContactResponse?> HandleAsync(UpdateAirlineContactCommand command, CancellationToken ct)
    {
        var contact = await db.Query<AirlineContact>().FirstOrDefaultAsync(c => c.Id == command.Id && c.AirlineCode == command.AirlineCode, ct);
        if (contact is null) return null;

        var r = command.Request;
        contact.Update(Enum.Parse<AirlineContactType>(r.ContactType, true), r.Value);
        await db.SaveChangesAsync(ct);
        return ListAirlineContactsQueryHandler.ToResponse(contact);
    }
}

public sealed class DeleteAirlineContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteAirlineContactCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteAirlineContactCommand command, CancellationToken ct)
    {
        var contact = await db.Query<AirlineContact>().FirstOrDefaultAsync(c => c.Id == command.Id && c.AirlineCode == command.AirlineCode, ct);
        if (contact is null) return false;

        db.Remove(contact);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

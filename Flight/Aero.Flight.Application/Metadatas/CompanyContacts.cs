using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Application.Metadatas;

public sealed record ListCompanyContactsQuery(string CompanyCode);
public sealed record AddCompanyContactCommand(string CompanyCode, CompanyContactRequest Request);
public sealed record UpdateCompanyContactCommand(string CompanyCode, int Id, CompanyContactRequest Request);
public sealed record DeleteCompanyContactCommand(string CompanyCode, int Id);

public sealed class ListCompanyContactsQueryHandler(IAeroFlightDbContext db)
    : IQueryHandler<ListCompanyContactsQuery, IReadOnlyList<CompanyContactResponse>>
{
    public async Task<IReadOnlyList<CompanyContactResponse>> HandleAsync(ListCompanyContactsQuery query, CancellationToken ct)
    {
        var items = await db.Query<CompanyContact>()
            .Where(c => c.CompanyCode == query.CompanyCode)
            .OrderBy(c => c.Id)
            .ToListAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    internal static CompanyContactResponse ToResponse(CompanyContact c) =>
        new(c.Id, c.CompanyCode, c.ContactType.ToString(), c.Value);
}

public sealed class AddCompanyContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<AddCompanyContactCommand, CompanyContactResponse>
{
    public async Task<CompanyContactResponse> HandleAsync(AddCompanyContactCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var contact = CompanyContact.Create(command.CompanyCode, Enum.Parse<CompanyContactType>(r.ContactType, true), r.Value);
        db.Add(contact);
        await db.SaveChangesAsync(ct);
        return ListCompanyContactsQueryHandler.ToResponse(contact);
    }
}

public sealed class UpdateCompanyContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<UpdateCompanyContactCommand, CompanyContactResponse?>
{
    public async Task<CompanyContactResponse?> HandleAsync(UpdateCompanyContactCommand command, CancellationToken ct)
    {
        var contact = await db.Query<CompanyContact>().FirstOrDefaultAsync(c => c.Id == command.Id && c.CompanyCode == command.CompanyCode, ct);
        if (contact is null) return null;

        var r = command.Request;
        contact.Update(Enum.Parse<CompanyContactType>(r.ContactType, true), r.Value);
        await db.SaveChangesAsync(ct);
        return ListCompanyContactsQueryHandler.ToResponse(contact);
    }
}

public sealed class DeleteCompanyContactCommandHandler(IAeroFlightDbContext db)
    : ICommandHandler<DeleteCompanyContactCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteCompanyContactCommand command, CancellationToken ct)
    {
        var contact = await db.Query<CompanyContact>().FirstOrDefaultAsync(c => c.Id == command.Id && c.CompanyCode == command.CompanyCode, ct);
        if (contact is null) return false;

        db.Remove(contact);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

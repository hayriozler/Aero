using Aero.Flights.Domain.Common;
using Aero.Shared.Domain.Identity;
using Aero.Shared.Infrastructure.Persistence;
using Aero.TextMessageParser.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Persistence;

public class TextMessageParserDbContext(DbContextOptions<TextMessageParserDbContext> options)
    : DataStoreContext(options), IDataStoreContext
{
    public DbSet<TextMessage> TextMessagesSet => Set<TextMessage>();
    public DbSet<TextMessageError> TextMessageErrorSet => Set<TextMessageError>();
    public DbSet<ElementError> TextMessageElementErrorSet => Set<ElementError>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<User>();
        modelBuilder.Ignore<Role>();
        modelBuilder.Ignore<UserRole>();
        modelBuilder.Ignore<Permission>();
        modelBuilder.Ignore<RolePermission>();
        modelBuilder.Ignore<RefreshToken>();
        modelBuilder.Ignore<AeroConfiguration>();
    }
}

using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal sealed class TestParserDbContext(DbContextOptions<TextMessageParserDbContext> options)
    : TextMessageParserDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TextMessage).Assembly);
        var foreign = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.ClrType.Assembly != typeof(TextMessage).Assembly)
            .Select(e => e.ClrType)
            .ToList();
        foreach (var type in foreign) modelBuilder.Ignore(type);
    }
}

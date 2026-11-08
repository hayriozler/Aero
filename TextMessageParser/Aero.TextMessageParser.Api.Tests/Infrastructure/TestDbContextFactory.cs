using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class TestDbContextFactory
{
    private static DbContextOptions<TextMessageParserDbContext> Options(string? databaseName) =>
        new DbContextOptionsBuilder<TextMessageParserDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

    public static TextMessageParserDbContext Create(string? databaseName = null)
        => new TestParserDbContext(Options(databaseName));

    public static TextMessageParserDbContext CreateProductionContext()
        => new TextMessageParserDbContext(Options(null));
}

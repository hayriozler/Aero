using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.TextMessageParser.Migrations;

internal static class TextMessageParserDbContextOptionsFactory
{
    public const string HistorySchema = "messages";

    public static DbContextOptions<TextMessageParserDbContext> Create(string connectionString)
        => new DbContextOptionsBuilder<TextMessageParserDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsAssembly(typeof(TextMessageParserDbContextOptionsFactory).Assembly.GetName().Name)
                .MigrationsHistoryTable("__EFMigrationsHistory", HistorySchema))
            .Options;
}

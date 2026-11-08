using Aero.TextMessageParser.Api.Persistence;
using Microsoft.EntityFrameworkCore.Design;

namespace Aero.TextMessageParser.Migrations;

internal sealed class TextMessageParserDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TextMessageParserDbContext>
{
    public TextMessageParserDbContext CreateDbContext(string[] args)
        => new(TextMessageParserDbContextOptionsFactory.Create(
            "Host=localhost;Database=textmessageparser_design;Username=postgres;Password=postgres"));
}

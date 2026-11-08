using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(TextMessageParserDbContextOptionsFactory).Assembly, optional: true)
    .AddCommandLine(args)
    .Build();

var connectionString = configuration.GetConnectionString("textmessageparser");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("Connection string 'textmessageparser' is not configured. Pass --ConnectionStrings:textmessageparser=<value>, set ConnectionStrings__textmessageparser or add it to the user secrets.");
    return 1;
}

await using var db = new TextMessageParserDbContext(TextMessageParserDbContextOptionsFactory.Create(connectionString));
var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
Console.WriteLine(pending.Count == 0
    ? "The database is up to date."
    : $"Applying {pending.Count} migration(s): {string.Join(", ", pending)}");
await db.Database.MigrateAsync();
Console.WriteLine("Done.");
return 0;

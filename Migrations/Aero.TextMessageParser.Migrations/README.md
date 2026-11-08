# Aero.TextMessageParser.Migrations

EF Core migrations for `TextMessageParserDbContext` (schema `messages`: `TextMessages`, `MessageErrors`, `ElementErrors`). The history table is `messages.__EFMigrationsHistory`.

The project is a console application so that it can both generate migrations (`dotnet ef`) and apply them.

## Add a migration

Run from the solution root. Build first, `--no-build` uses the last build output.

```
dotnet build Migrations/Aero.TextMessageParser.Migrations
dotnet ef migrations add <Name> --project Migrations/Aero.TextMessageParser.Migrations --output-dir Migrations --no-build
```

The design time factory uses a dummy connection string, no database is needed to add a migration. Rebuild before running the application, otherwise the new migration is not found.

## Apply migrations

The connection string is read from, in order of precedence, the command line, the user secrets and the environment variables.

```
dotnet run --project Migrations/Aero.TextMessageParser.Migrations -- "--ConnectionStrings:textmessageparser=Host=localhost;Database=textmessageparser;Username=postgres;Password=<password>"
```

The same value can be set as `ConnectionStrings__textmessageparser` or with `dotnet user-secrets set ConnectionStrings:textmessageparser "<value>"` in the project folder.

## A database that already has the tables

`InitialCreate` creates all three tables. If they were created by hand, either apply the migration to an empty database, or mark it as applied without running it:

```
INSERT INTO messages."__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ('20261108154257_InitialCreate', '10.0.14');
```

The history table must exist first (it is created when the tool connects to the database), and the existing columns must match the model: the owned header and footer are the columns `Header_Identifier`, `Header_SecondaryIdentifier`, `Header_ChangeOfStatus` and `Footer_EndIdentifier`.

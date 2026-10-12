cd "D:\SourceCodes\Z&M\AeroFlight"
dotnet ef migrations add "InitialMigration" --project src/AeroFlight.Migrations --output-dir Migrations/Tenant --context TenantDbContext
dotnet ef migrations add "InitialMigration" --project src/AeroFlight.Migrations --output-dir Migrations/Airport --context AeroFlightDbContext
dotnet ef migrations add "InitialMigration" --project src/AeroFlight.Migrations --output-dir Migrations/Fids --context FidsDbContext
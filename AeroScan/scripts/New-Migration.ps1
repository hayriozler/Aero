param(
	[string] $MigrationName = "InitialCreate",
	[string] $Project = "BaggageService",
	[string] $StartupProject = "BaggageService",
	[string] $Context = "AeroScanDataContext",
	[string] $OutputDir = "Persistence/Migrations",
	[switch] $Apply
)

Write-Host "Creating EF Core migration '$MigrationName' for context '$Context'..."

# Ensure solution builds
Write-Host "Building solution..."
dotnet build || { Write-Error "Build failed. Fix compile errors before creating migrations."; exit 1 }

# Ensure dotnet-ef is available
$ef = dotnet tool list -g | Select-String "dotnet-ef"
if (-not $ef) {
	Write-Host "dotnet-ef global tool not found. Installing..."
	dotnet tool install --global dotnet-ef || { Write-Error "Failed to install dotnet-ef. Install manually and re-run the script."; exit 1 }
}

Write-Host "Adding migration..."
dotnet ef migrations add $MigrationName --project $Project --startup-project $StartupProject --context $Context --output-dir $OutputDir || { Write-Error "dotnet ef failed to add migration."; exit 1 }

if ($Apply) {
	Write-Host "Applying migration to database..."
	dotnet ef database update --project $Project --startup-project $StartupProject --context $Context || { Write-Error "Failed to apply migration to database."; exit 1 }
}

Write-Host "Migration created. Files are in $Project\$OutputDir"

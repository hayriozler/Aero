namespace Aero.Migrations.Options;

public sealed class SeedOptions
{
    public const string Section = "Seed";

    public string AdminUsername { get; init; } = "admin";
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminRealName { get; init; } = "System Administrator";

    public string NormalUsername { get; init; } = "operator";
    public string NormalPassword { get; init; } = string.Empty;
    public string NormalRealName { get; init; } = "Terminal Operator";

    public string ReadOnlyUsername { get; init; } = "viewer";
    public string ReadOnlyPassword { get; init; } = string.Empty;
    public string ReadOnlyRealName { get; init; } = "Read-Only Viewer";

    public string SourceConnectionString { get; init; } = string.Empty;
}

using System.Text.Json;

namespace Aero.Scanner.Mobile.Config;

public sealed record AppConfig(
    string HhtName,
    string GatewayUrl,
    string ExitPassword,
    string Language)
{
    public static AppConfig Load()
    {
        var asm = typeof(AppConfig).Assembly;
        using var stream = asm.GetManifestResourceStream("Aero.Scanner.Mobile.appsettings.json")
            ?? throw new InvalidOperationException("appsettings.json not found as embedded resource.");

        var config = JsonSerializer.Deserialize<AppConfig>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("appsettings.json could not be deserialized.");

        return config;
    }

    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(HhtName))
            errors.Add("HhtName must be set in appsettings.json");
        if (string.IsNullOrWhiteSpace(GatewayUrl))
            errors.Add("GatewayUrl must be set in appsettings.json");
        return errors;
    }
}

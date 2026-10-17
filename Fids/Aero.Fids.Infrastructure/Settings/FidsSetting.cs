namespace Aero.Fids.Infrastructure.Settings;

public sealed class FidsSetting
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private FidsSetting() { }

    public static FidsSetting Create(string key, string value) => new() { Key = key, Value = value };

    public void UpdateValue(string value) => Value = value;
}

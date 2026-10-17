namespace Aero.Flights.Domain.Common;

public sealed class AeroConfiguration : AuditBase<int>
{
    public string ConfigurationName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Value { get; private set; }
    public string? ConfigType { get; private set; }
    public bool IsVisible { get; private set; }
    public bool IsUpdateable { get; private set; }

    private AeroConfiguration() { }

    private AeroConfiguration(string configurationName, string? value, string? configType, bool isVisible, bool isUpdateable)
    {
        ConfigurationName = configurationName;
        Value = value;
        ConfigType = configType;
        IsVisible = isVisible;
        IsUpdateable = isUpdateable;
    }

    public static AeroConfiguration Create(string configurationName, string? value, string? configType, bool isVisible = true, bool isUpdateable = true)
        => new(configurationName, value, configType, isVisible, isUpdateable);

    public void UpdateValue(string value) => Value = value;
    public void UpdateDescription(string description) => Description = description;
}

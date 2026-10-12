using Domain.Common;

namespace Domain.Aggregates.Settings;

public sealed class SystemConfiguration : AuditableEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public static SystemConfiguration Create(string key, string value, string? description = null) => new()
    {
        Key = key,
        Value = value,
        Description = description
    };

    public SystemConfiguration Update(string? value, string? description = null) 
    {
        var systemConfiguration = new SystemConfiguration();
        if (!string.IsNullOrEmpty(value))
            systemConfiguration.Value = value;  
        systemConfiguration.Description = description;
        return systemConfiguration;
    }
}
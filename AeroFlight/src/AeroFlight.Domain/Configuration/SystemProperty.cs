using AeroFlight.Domain.Common;

namespace AeroFlight.Domain.Configuration;

public sealed class SystemProperty : AuditBase<string>
{
    public string Code { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private SystemProperty() { }

    private SystemProperty(string code, string value)
    {
        SetId(code);
        Code = code;
        Value = value;
    }

    public static SystemProperty Create(string code, string value) => new(code, value);

    public void UpdateValue(string value) => Value = value;
}

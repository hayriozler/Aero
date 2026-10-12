using Domain.Common;

namespace Domain.Aggregates.Mappings;

public sealed class AirlineClassMap : CompositeEntity
{
    public string AirlineCode { get; private set; } = string.Empty;
    public char SourceClass { get; private set; }
    public char TargetClass { get; private set; } 

    private AirlineClassMap() { }

    public static AirlineClassMap Create(string airlineCode, char sourceClass, char targetClass) => new()
    {
        AirlineCode = airlineCode.ToUpperInvariant().Trim(),
        SourceClass = sourceClass,
        TargetClass = targetClass
    };

    public void Update(char targetClass) => TargetClass = targetClass;
}

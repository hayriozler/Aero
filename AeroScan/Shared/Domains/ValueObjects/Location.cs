namespace Domain.ValueObjects;

public readonly record struct Location(string Code, string? Description = null)
{
    public override string ToString() => Code;
}

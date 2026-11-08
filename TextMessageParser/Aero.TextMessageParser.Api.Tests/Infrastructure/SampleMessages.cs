namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class SampleMessages
{
    public static readonly string NativeNewLine = Environment.NewLine;

    public const string V = ".V/7LFRA";
    public const string F = ".F/LH400/12NOV/JFK/Y";
    public const string N1 = ".N/0220123456001";
    public const string N2 = ".N/0220123457001";
    public const string N3 = ".N/0220123458001";
    public const string P = ".P/DOE/JOHN";

    public static string Build(string type, string? subType, string newLine, params string[] body)
    {
        var lines = new List<string> { type };
        if (subType is not null) lines.Add(subType);
        lines.AddRange(body);
        lines.Add("END" + type);
        return string.Join(newLine, lines);
    }

    public static string BsmText(params string[] body) => Build("BSM", null, NativeNewLine, body);
    public static string ChgText(params string[] body) => Build("BSM", "CHG", NativeNewLine, body);
    public static string DelText(params string[] body) => Build("BSM", "DEL", NativeNewLine, body);
    public static string BcmText(string subType, params string[] body) => Build("BCM", subType, NativeNewLine, body);

    public static string SingleTagBsm() => BsmText(V, F, N1, P);
}

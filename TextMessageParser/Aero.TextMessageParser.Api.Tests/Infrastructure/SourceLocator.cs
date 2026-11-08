namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class SourceLocator
{
    public static string ReadApiSource(string relativePath)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Aero.slnx"))) dir = dir.Parent;
        if (dir is null) throw new InvalidOperationException("Aero.slnx not found above " + AppContext.BaseDirectory);
        var path = Path.Combine(dir.FullName, "TextMessageParser", "Aero.TextMessageParser.Api", relativePath);
        return File.ReadAllText(path);
    }
}

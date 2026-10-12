using System.Web;

namespace Aero.Scanner.WebApp.Services;

public abstract class FlightApiClientBase
{
    protected static string BuildUrl(string base_, bool includeAll, DateTime? from, DateTime? to)
    {
        var qs = HttpUtility.ParseQueryString(string.Empty);
        if (includeAll) qs["includeAll"] = "true";
        if (from.HasValue) qs["from"]       = from.Value.ToString("yyyy-MM-dd");
        if (to.HasValue) qs["to"]         = to.Value.ToString("yyyy-MM-dd");
        var query = qs.ToString();
        return string.IsNullOrEmpty(query) ? base_ : $"{base_}?{query}";
    }
}

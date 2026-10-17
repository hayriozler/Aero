namespace Aero.Fids.Web.Boards;

internal static class BoardGroups
{
    public static string NameForList(string tenantId, string type) =>
        $"{tenantId.ToLowerInvariant()}:board:{type.ToLowerInvariant()}";

    public static string NameForResource(string tenantId, string type, string code) =>
        $"{tenantId.ToLowerInvariant()}:resource:{type.ToLowerInvariant()}:{code.ToLowerInvariant()}";

    public static string NameForResourceOverview(string tenantId, string type) =>
        $"{tenantId.ToLowerInvariant()}:resource-overview:{type.ToLowerInvariant()}";
}

namespace AeroFlight.Contracts.Identity;

// The claim type carrying the tenant id — written by both the JWT (API auth, JwtTokenService) and
// cookie (Web session, AuthEndpoints) issuers, and read by both the Web client (to route each API
// call under the right tenant URL segment) and the Api's tenant-access filter. A mismatch between
// writer and reader here would silently break tenant resolution with no compiler signal, which is
// why this is one shared constant instead of the literal "tenant" typed out at each site.
public static class TenantClaimTypes
{
    public const string Tenant = "tenant";
}

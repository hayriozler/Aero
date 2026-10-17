namespace Aero.Flights.Contracts.Flights;

// Last-resort fallback when a tenant has no "Resources.{Kind}.DefaultDurationMinutes" AeroConfig
// row at all (normally seeded by AeroConfigSeeder — see its per-kind values there for the real
// tunable defaults). Shared by every layer that reads resource duration defaults so they can't
// silently drift apart.
public static class ResourceDefaultsConstants
{
    public const int FallbackDurationMinutes = 120;
}

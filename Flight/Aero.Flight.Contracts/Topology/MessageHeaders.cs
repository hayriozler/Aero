namespace Aero.Flights.Contracts.Topology;

// AMQP message-property header keys — distinct from TenantClaimTypes (HTTP auth claims): these
// only carry meaning inside a RabbitMQ message's BasicProperties.Headers, not an ASP.NET
// ClaimsPrincipal. They happen to share the literal "tenant" today, but are free to diverge since
// they're two different protocols.
public static class MessageHeaders
{
    public const string Tenant = "tenant";
}

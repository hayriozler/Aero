namespace AeroFlight.Contracts.Topology;

public static class RoutingKeys
{
    public static class Outbound
    {
        public const string FlightStatusChanged = "flight.status.changed";
        public const string FlightArrivalCreated = "flight.arrival.created";
        public const string FlightDepartureCreated = "flight.departure.created";
        public const string FlightArrivalUpdated = "flight.arrival.updated";
        public const string FlightDepartureUpdated = "flight.departure.updated";
        public const string FlightGateAssigned = "flight.gate.assigned";
        public const string FlightBeltAssigned = "flight.belt.assigned";
        public const string FlightCounterAssigned = "flight.counter.assigned";
        public const string FlightDelayAdded = "flight.delay.added";
        public const string FlightCancelled = "flight.cancelled";
        public const string FlightDiverted = "flight.diverted";
    }

    public static class Inbound
    {
        public const string ScheduleArrivalReceived = "schedule.arrival.received";
        public const string ScheduleDepartureReceived = "schedule.departure.received";
        public const string MvtReceived = "mvt.received";
        public const string LdmReceived = "ldm.received";
        public const string MovementMessageReceived = "movement.message.received";

        // External master-data push; payload is a MetadataDataChanged envelope. Casing here is
        // "Metadata.data.changed" (not lowercase like every other key in this file) — flagged by an
        // earlier audit as inconsistent, but left as-is rather than changed: this is the literal
        // wire-level RabbitMQ routing key an external upstream system publishes to, and routing keys
        // are case-sensitive, so "fixing" the casing would silently break that integration unless
        // the upstream publisher is changed in lockstep. Don't casually normalize this string.
        public const string MetadataDataChanged = "Metadata.data.changed";

        public const string ArrivalFlightCreated = "flight.arrival.created";
        public const string DepartureFlightCreated = "flight.departure.created";
        public const string ArrivalFlightDeleted = "flight.arrival.deleted";
        public const string DepartureFlightDeleted = "flight.departure.deleted";
        public const string ArrivalResourceChanged = "flight.arrival.resource.changed";
        public const string DepartureResourceChanged = "flight.departure.resource.changed";
        public const string ArrivalFlightUpdated = "flight.arrival.updated";
        public const string DepartureFlightUpdated = "flight.departure.updated";
    }

    public static class MetadataData
    {
        public const string Created = "created";
        public const string Updated = "updated";
        public const string Deleted = "deleted";

        public static string For(string entity, string operation) => $"{entity.ToLowerInvariant()}.{operation}";
    }
}

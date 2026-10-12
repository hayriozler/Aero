namespace AeroFlight.Web.Pages.Flights;

public partial class FlightEditPanel
{
    private sealed class DetailEditModel
    {
        public string FlightNumber { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public DateTime? ScheduledDate { get; set; }
        public TimeSpan? ScheduledTime { get; set; }
        public string AirportCode { get; set; } = "";
        public string CallSign { get; set; } = "";
        public string OperatingAirlineCode { get; set; } = "";
        public string FlightTypeCode { get; set; } = "";
        public string AircraftCode { get; set; } = "";
        public string AircraftTypeCode { get; set; } = "";
        public string TerminalCode { get; set; } = "";
        public string HandlingAgentCode { get; set; } = "";
        public string Comment { get; set; } = "";
        public int? PaxTotal { get; set; }
        public int? CodeshareNumber { get; set; }
        public string CountryType { get; set; } = "";
        public string ParkPositionCode { get; set; } = "";
        public string? RoutingSummary { get; set; }

        public string DivertAirportCode { get; set; } = "";

        public DateTime? EtaDate { get; set; }
        public TimeSpan? EtaTime { get; set; }
        public DateTime? AtaDate { get; set; }
        public TimeSpan? AtaTime { get; set; }
        public DateTime? OnBlockDate { get; set; }
        public TimeSpan? OnBlockTime { get; set; }

        public DateTime? EtdDate { get; set; }
        public TimeSpan? EtdTime { get; set; }
        public DateTime? AtdDate { get; set; }
        public TimeSpan? AtdTime { get; set; }
        public DateTime? OffBlockDate { get; set; }
        public TimeSpan? OffBlockTime { get; set; }

        public int? PaxTransit { get; set; }
        public int? PaxAssistanceRequired { get; set; }
        public int? Pax1 { get; set; }
        public int? Pax2 { get; set; }
        public int? Pax3 { get; set; }
        public int? CrewNumber { get; set; }
        public int? CrewCockpit { get; set; }
        public decimal? BaggageWeight { get; set; }
        public decimal? BaggageTotal { get; set; }
        public decimal? CargoWeight { get; set; }
        public decimal? MailWeight { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? EquipmentWeight { get; set; }
        public decimal? CabinLuggage { get; set; }
    }

    private sealed record MetadataOption(string Code, string Label);
    private sealed record FlightStatusOption(string Code, string Label, bool IsUpdatable);
}

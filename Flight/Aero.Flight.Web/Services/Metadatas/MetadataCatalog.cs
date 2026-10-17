using MudBlazor;

namespace Aero.Flights.Web.Services.Metadatas;

public enum RefFieldType { Text, Number, Decimal, Bool, Select, Autocomplete }

public sealed record RefField(string Name, string Label, RefFieldType Type, bool Required = false, int? MaxLength = null, string? OptionsRoute = null);

public sealed record RefColumn(string Name, string Label);

public sealed record MetadataEntity(
    string Key,
    string Title,
    string Group,
    string Icon,
    string Route,
    bool ReadOnly,
    IReadOnlyList<RefColumn> Columns,
    IReadOnlyList<RefField> Fields);

public static class MetadataCatalog
{
    private const string _layoutGroup = "Airport Layout";
    private const string _baggageGroup = "Baggage & Check-in";
    private const string _fleetGroup = "Airlines & Aircraft";
    private const string _geoGroup = "Geography";
    private const string _opsGroup = "Operations";
    private const string _statusGroup = "Statuses";
    private const string _lookupGroup = "Lookups";

    public static readonly IReadOnlyList<MetadataEntity> All =
    [
        // ── Airport Layout ──────────────────────────────────────────────
        new("terminals", "Terminals", _layoutGroup, Icons.Material.Filled.Business, "/api/metadata/terminals", false,
            [C("code", "Code"), C("description", "Description")],
            [T("code", "Code", true, 10), T("description", "Description", false, 200)]),

        new("gates", "Gates", _layoutGroup, Icons.Material.Filled.SensorDoor, "/api/metadata/gates", false,
            [C("code", "Code"),  C("gateTypeCode", "Type"), C("terminalCode", "Terminal"), C("description", "Description"), C("capacity", "Capacity")],
            [T("code", "Code", true, 10), T("gateTypeCode", "Gate Type Code", false, 10),
             S("terminalCode", "Terminal", "/api/metadata/terminals"),
             T("description", "Description", false, 200), N("capacity", "Capacity"),
             B("hasLoadingBridge", "Has Loading Bridge"), B("hasPca", "Has PCA")]),

        new("parkpositions", "Parking Positions", _layoutGroup, Icons.Material.Filled.LocalParking, "/api/metadata/parkpositions", false,
            [C("code", "Code"),  C("parkingPositionTypeCode", "Type"), C("description", "Description"), C("capacity", "Capacity")],
            [T("code", "Code", true, 10), T("parkingPositionTypeCode", "Parking Position Type Code", false, 10),
             T("description", "Description", false, 200), N("capacity", "Capacity"),T("airlineCode", "Airline Code", false, 3),
             D("latitude", "Latitude"), D("longitude", "Longitude"), D("angle", "Angle")]),

        new("runways", "Runways", _layoutGroup, Icons.Material.Filled.Straighten, "/api/metadata/runways", false,
            [C("code", "Code"), C("name", "Name"), C("length", "Length"), C("width", "Width")],
            [T("code", "Code", true, 10), T("name", "Name", false, 100), D("length", "Length"), D("width", "Width")]),

        // ── Baggage & Check-in ──────────────────────────────────────────
        new("arrivalbelts", "Arrival Belts", _baggageGroup, Icons.Material.Filled.Luggage, "/api/metadata/arrivalbelts", false,
            [C("code", "Code"), C("terminalCode", "Terminal"), C("description", "Description"), C("capacity", "Capacity")],
            [T("code", "Code", true, 10), S("terminalCode", "Terminal", "/api/metadata/terminals"),
             T("description", "Description", false, 200), N("capacity", "Capacity")]),

        new("departurebelts", "Departure Belts", _baggageGroup, Icons.Material.Filled.Luggage, "/api/metadata/departurebelts", false,
            [C("code", "Code"), C("terminalCode", "Terminal"), C("description", "Description"), C("capacity", "Capacity")],
            [T("code", "Code", true, 10), S("terminalCode", "Terminal", "/api/metadata/terminals"),
             T("description", "Description", false, 200), N("capacity", "Capacity")]),

        new("counters", "Counters", _baggageGroup, Icons.Material.Filled.TableRows, "/api/metadata/counters", false,
            [C("code", "Code"), C("terminalCode", "Terminal"), C("description", "Description"), C("capacity", "Capacity")],
            [T("code", "Code", true, 10), S("terminalCode", "Terminal", "/api/metadata/terminals"),
             T("description", "Description", false, 200), N("capacity", "Capacity")]),

        new("checkinclasses", "Check-in Classes", _baggageGroup, Icons.Material.Filled.Category, "/api/metadata/checkinclasses", false,
            [C("code", "Code"), C("description", "Description")],
            [T("code", "Code", true, 10), T("description", "Description", true, 50)]),


        // ── Airlines & Aircraft ─────────────────────────────────────────
        new("airlines", "Airlines", _fleetGroup, Icons.Material.Filled.Flight, "/api/metadata/airlines", false,
            [C("code", "Code"), C("iataCode", "IATA"), C("icaoCode", "ICAO"), C("name", "Name"), C("countryCode", "Country")],
            [T("code", "Code", true, 10), T("iataCode", "IATA Code", true, 3),
             T("icaoCode", "ICAO Code", false, 4), T("callsign", "Callsign", false, 50),
             T("name", "Name", false, 100), T("displayName", "Display Name", false, 100),
             A("parentAirlineCode", "Parent Airline Code", "/api/metadata/airlines"), S("countryCode", "Country Code", "/api/metadata/countries")]),

        new("companies", "Companies", _fleetGroup, Icons.Material.Filled.SupportAgent, "/api/metadata/companies", false,
            [C("code", "Code"), C("name", "Name"), C("displayName", "Display Name"), C("companyTypeCode", "Type")],
            [T("code", "Code", true, 10), T("name", "Name", false, 100), T("displayName", "Display Name", false, 100),
             S("companyTypeCode", "Company Type", "/api/metadata/companytypes"),
             T("taxNumber", "Tax Number", false, 30), T("billingAddress", "Billing Address", false, 500),
             T("bankAccount", "Bank Account (IBAN)", false, 50), T("invoiceCurrency", "Invoice Currency", false, 3)]),

        new("companytypes", "Company Types", _fleetGroup, Icons.Material.Filled.Category, "/api/metadata/companytypes", false,
            [C("code", "Code"), C("description", "Description")],
            [T("code", "Code", true, 10), T("description", "Description", true, 50)]),

        new("aircraft", "Aircraft", _fleetGroup, Icons.Material.Filled.AirplanemodeActive, "/api/metadata/aircraft", false,
            [C("code", "Code"), C("aircraftTypeCode", "Type"), C("airlineCode", "Airline"), C("owner", "Owner"), C("seatCapacity", "Seats")],
            [T("code", "Code", true, 20), T("aircraftTypeCode", "Aircraft Type Code", true, 10), A("airlineCode", "Airline Code", "/api/metadata/airlines"),
             T("owner", "Owner", false, 100),
             S("sizeCategory", "Size Category", "/api/metadata/aircraftsizecategories"), S("noiseCategory", "Noise Category", "/api/metadata/aircraftnoisecategories"),
             D("billWeight", "Bill Weight"), N("seatCapacity", "Seat Capacity")]),

        new("aircrafttypes", "Aircraft Types", _fleetGroup, Icons.Material.Filled.FlightClass, "/api/metadata/aircrafttypes", false,
            [C("code", "Code"), C("iataCode", "IATA"), C("icaoCode", "ICAO"), C("manufacturer", "Manufacturer"), C("model", "Model")],
            [T("code", "Code", true, 20), T("iataCode", "IATA Code", true, 5), T("iataSubCode", "IATA Sub Code", false, null),
             T("icaoCode", "ICAO Code", false, 4), S("sizeCategory", "Size Category", "/api/metadata/aircraftsizecategories"), S("noiseCategory", "Noise Category", "/api/metadata/aircraftnoisecategories"),
             T("manufacturer", "Manufacturer", false, 100), T("model", "Model", false, 100), D("mtow", "MTOW"),
             D("landingWeight", "Landing Weight"), N("avgSeatCapacity", "Avg Seat Capacity"), B("isBridgeRequired", "Bridge Required")]),

        new("aircraftsizecategories", "Aircraft Size Categories", _fleetGroup, Icons.Material.Filled.PhotoSizeSelectSmall, "/api/metadata/aircraftsizecategories", false,
            [C("code", "Code"), C("description", "Description")],
            [T("code", "Code", true, 20), T("description", "Description", true, 200)]),

        // ── Geography ───────────────────────────────────────────────────
        new("airports", "Airports", _geoGroup, Icons.Material.Filled.ConnectingAirports, "/api/metadata/airports", false,
            [C("code", "Code"), C("iataCode", "IATA"), C("icaoCode", "ICAO"), C("name", "Name"), C("countryCode", "Country")],
            [T("code", "Code", true, 10), T("iataCode", "IATA Code", false, 3),
             T("icaoCode", "ICAO Code", false, 4), T("name", "Name", false, 100),
             T("displayName", "Display Name", false, 100), T("location", "Location", false, 200),
             S("countryCode", "Country Code", "/api/metadata/countries"),
             T("iataRegionCode", "IATA Region Code", false, 10), S("timezoneCode", "Timezone", "/api/metadata/timezones")]),

        new("countries", "Countries", _geoGroup, Icons.Material.Filled.Flag, "/api/metadata/countries", false,
            [C("code", "Code"), C("name", "Name"), C("countryTypeCode", "Type")],
            [T("code", "Code", true, 10), T("name", "Name", true, 200), S("countryTypeCode", "Country Type Code", "/api/metadata/countrytypes")]),

        // ── Operations ──────────────────────────────────────────────────
        new("delayreasons", "Delay Reasons", _opsGroup, Icons.Material.Filled.Timer, "/api/metadata/delayreasons", false,
            [C("code", "Code"), C("alphabeticCode", "Alpha"), C("description", "Description")],
            [T("code", "Code", true, 3), T("alphabeticCode", "Alphabetic Code", false, 3), T("description", "Description", false, 200)]),

        Lookup("flighttypes", "Flight Types", "/api/metadata/flighttypes", Icons.Material.Filled.Category, _opsGroup),

        new("handlingservices", "Handling Services", _opsGroup, Icons.Material.Filled.Handyman, "/api/metadata/handlingservices", false,
            [C("code", "Code"), C("description", "Description"), C("unit", "Unit")],
            [T("code", "Code", true, 10), T("description", "Description", false, 200), T("unit", "Unit", false, 20)]),


        new("remarks", "Remarks", _opsGroup, Icons.Material.Filled.Comment, "/api/metadata/remarks", false,
            [C("code", "Code"), C("description", "Description")],
            [T("code", "Code", true, 20), T("description", "Description", false, 50)]),

        // ── Statuses (read-only) ────────────────────────────────────────
        Lookup("arrivalbeltstatuses", "Arr. Belt Statuses", "/api/metadata/arrivalbeltstatuses", Icons.Material.Filled.Circle),
        Lookup("departurebeltstatuses", "Dep. Belt Statuses", "/api/metadata/departurebeltstatuses", Icons.Material.Filled.Circle),
        Lookup("counterstatuses", "Counter Statuses", "/api/metadata/counterstatuses", Icons.Material.Filled.Circle),
        Lookup("deboardingstatuses", "DeBoarding Statuses", "/api/metadata/deboardingstatuses", Icons.Material.Filled.Circle),
        Lookup("flightstatuses", "Flight Statuses", "/api/metadata/flightstatuses", Icons.Material.Filled.Circle),
        Lookup("gatestatuses", "Gate Statuses", "/api/metadata/gatestatuses", Icons.Material.Filled.Circle),

        // ── Lookups (read-only) ─────────────────────────────────────────
        new("gatetypes", "Gate Types", _lookupGroup, Icons.Material.Filled.ListAlt, "/api/metadata/gatetypes", true,
            [C("code", "Code"), C("description", "Description")], []),
        Lookup("country-types", "Country Types", "/api/metadata/countrytypes", Icons.Material.Filled.ListAlt, _lookupGroup),
        Lookup("regions", "Regions", "/api/metadata/regions", Icons.Material.Filled.ListAlt, _lookupGroup),
        Lookup("parkposition-types", "Park Position Types", "/api/metadata/parkpositiontypes", Icons.Material.Filled.ListAlt, _lookupGroup),
        Lookup("aircraftnoisecategories", "Aircraft Noise Categories", "/api/metadata/aircraftnoisecategories", Icons.Material.Filled.ListAlt, _lookupGroup),
    ];

    public static MetadataEntity? ByKey(string key) =>
        All.FirstOrDefault(e => string.Equals(e.Key, key, StringComparison.OrdinalIgnoreCase));

    public static string RouteOf(string key) =>
        ByKey(key)?.Route ?? throw new InvalidOperationException($"MetadataCatalog has no entry for '{key}'.");

    public static IEnumerable<IGrouping<string, MetadataEntity>> Groups => All.GroupBy(e => e.Group);

    public static string GroupIcon(string group) => group switch
    {
        _layoutGroup => Icons.Material.Filled.Domain,
        _baggageGroup => Icons.Material.Filled.Luggage,
        _fleetGroup => Icons.Material.Filled.Flight,
        _geoGroup => Icons.Material.Filled.Public,
        _opsGroup => Icons.Material.Filled.Settings,
        _statusGroup => Icons.Material.Filled.Traffic,
        _ => Icons.Material.Filled.ListAlt,
    };

    private static MetadataEntity Lookup(string key, string title, string route, string icon, string group = _statusGroup) =>
        new(key, title, group, icon, route, true,
            [C("code", "Code"), C("description", "Description")], []);

    private static RefField T(string name, string label, bool required, int? maxLength) => new(name, label, RefFieldType.Text, required, maxLength);
    private static RefField N(string name, string label) => new(name, label, RefFieldType.Number);
    private static RefField D(string name, string label) => new(name, label, RefFieldType.Decimal);
    private static RefField B(string name, string label) => new(name, label, RefFieldType.Bool);
    private static RefField S(string name, string label, string optionsRoute, bool required = false) =>
        new(name, label, RefFieldType.Select, required, OptionsRoute: optionsRoute);
    private static RefField A(string name, string label, string optionsRoute, bool required = false) =>
        new(name, label, RefFieldType.Autocomplete, required, OptionsRoute: optionsRoute);
    private static RefColumn C(string name, string label) => new(name, label);
}

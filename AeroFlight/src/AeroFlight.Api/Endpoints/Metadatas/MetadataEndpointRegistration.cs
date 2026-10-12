using AeroFlight.Contracts.Metadatas;
using AeroFlight.Domain.Metadatas;

namespace AeroFlight.Api.Endpoints.Metadatas;

public static class MetadataEndpointRegistration
{
    public static IEndpointRouteBuilder MapMetadataEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapMetadataCrudEndpoints<Gate, GateRequest, GateResponse>("/api/metadata/gates", "Metadata: Gates", r => r.Code);
        app.MapMetadataCrudEndpoints<Airport, AirportRequest, AirportResponse>("/api/metadata/airports", "Metadata: Airports", r => r.Code);
        app.MapMetadataCrudEndpoints<Airline, AirlineRequest, AirlineResponse>("/api/metadata/airlines", "Metadata: Airlines", r => r.Code);
        app.MapMetadataCrudEndpoints<Company, CompanyRequest, CompanyResponse>("/api/metadata/companies", "Metadata: Companies", r => r.Code);
        app.MapMetadataCrudEndpoints<CompanyType, CompanyTypeRequest, CompanyTypeResponse>("/api/metadata/companytypes", "Metadata: Company Types", r => r.Code);
        app.MapMetadataCrudEndpoints<CheckinClass, CheckinClassRequest, CheckinClassResponse>("/api/metadata/checkinclasses", "Metadata: Check-in Classes", r => r.Code);
        app.MapMetadataCrudEndpoints<ArrivalBelt, ArrivalBeltRequest, ArrivalBeltResponse>("/api/metadata/arrivalbelts", "Metadata: Arrival Belts", r => r.Code);
        app.MapMetadataCrudEndpoints<Counter, CounterRequest, CounterResponse>("/api/metadata/counters", "Metadata: Counters", r => r.Code);
        app.MapMetadataCrudEndpoints<Country, CountryRequest, CountryResponse>("/api/metadata/countries", "Metadata: Countries", r => r.Code);
        app.MapMetadataCrudEndpoints<DelayReason, DelayReasonRequest, DelayReasonResponse>("/api/metadata/delayreasons", "Metadata: Delay Reasons", r => r.Code);
        app.MapMetadataCrudEndpoints<DepartureBelt, DepartureBeltRequest, DepartureBeltResponse>("/api/metadata/departurebelts", "Metadata: Departure Belts", r => r.Code);
        app.MapMetadataCrudEndpoints<Remark, RemarkRequest, RemarkResponse>("/api/metadata/remarks", "Metadata: Remarks", r => r.Code);
        app.MapMetadataCrudEndpoints<Runway, RunwayRequest, RunwayResponse>("/api/metadata/runways", "Metadata: Runways", r => r.Code);
        app.MapMetadataCrudEndpoints<ParkPosition, ParkPositionRequest, ParkPositionResponse>("/api/metadata/parkpositions", "Metadata: Park Positions", r => r.Code);
        app.MapMetadataCrudEndpoints<HandlingService, HandlingServiceRequest, HandlingServiceResponse>("/api/metadata/handlingservices", "Metadata: Handling Services", r => r.Code);
        app.MapMetadataCrudEndpoints<Terminal, TerminalRequest, TerminalResponse>("/api/metadata/terminals", "Metadata: Terminals", r => r.Code);
        app.MapMetadataCrudEndpoints<Aircraft, AircraftRequest, AircraftResponse>("/api/metadata/aircraft", "Metadata: Aircraft", r => r.Code);
        app.MapMetadataCrudEndpoints<AircraftType, AircraftTypeRequest, AircraftTypeResponse>("/api/metadata/aircrafttypes", "Metadata: Aircraft Types", r => r.Code);
        app.MapMetadataCrudEndpoints<AircraftSizeCategory, AircraftSizeCategoryRequest, AircraftSizeCategoryResponse>("/api/metadata/aircraftsizecategories", "Metadata: Aircraft Size Categories", r => r.Code);

        app.MapMetadataLookupEndpoints<ArrivalBeltStatus, ArrivalBeltStatusResponse>("/api/metadata/arrivalbeltstatuses", "Metadata: ArrivalBelt Statuses");
        app.MapMetadataLookupEndpoints<DepartureBeltStatus, DepartureBeltStatusResponse>("/api/metadata/departurebeltstatuses", "Metadata: DepartureBelt Statuses");
        app.MapMetadataLookupEndpoints<CounterStatus, CounterStatusResponse>("/api/metadata/counterstatuses", "Metadata: Counter Statuses");
        app.MapMetadataLookupEndpoints<CountryType, CountryTypeResponse>("/api/metadata/countrytypes", "Metadata: Country Types");
        app.MapMetadataLookupEndpoints<DeBoardingStatus, DeBoardingStatusResponse>("/api/metadata/deboardingstatuses", "Metadata: DeBoarding Statuses");
        app.MapMetadataLookupEndpoints<FlightStatus, FlightStatusResponse>("/api/metadata/flightstatuses", "Metadata: Flight Statuses");
        app.MapMetadataLookupEndpoints<FlightType, FlightTypeResponse>("/api/metadata/flighttypes", "Metadata: Flight Types");
        app.MapMetadataLookupEndpoints<GateStatus, GateStatusResponse>("/api/metadata/gatestatuses", "Metadata: Gate Statuses");
        app.MapMetadataLookupEndpoints<GateType, GateTypeResponse>("/api/metadata/gatetypes", "Metadata: Gate Types");
        app.MapMetadataLookupEndpoints<ParkPositionType, ParkPositionTypeResponse>("/api/metadata/parkpositiontypes", "Metadata: Park Position Types");
        app.MapMetadataLookupEndpoints<Region, RegionResponse>("/api/metadata/regions", "Metadata: Regions");
        app.MapMetadataLookupEndpoints<AircraftNoiseCategory, AircraftNoiseCategoryResponse>("/api/metadata/aircraftnoisecategories", "Metadata: Aircraft Noise Categories");
        app.MapMetadataLookupEndpoints<Timezone, TimezoneResponse>("/api/metadata/timezones", "Metadata: Time Zones");

        return app;
    }
}

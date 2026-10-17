using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Configuration;
using Aero.Flights.Domain.Messaging;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Delays;
using Aero.Flights.Domain.Ops.MovementMessages;
using Aero.Flights.Domain.Ops.Passengers;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Ops.Routing;
using Aero.Flights.Domain.Ops.Services;
using Aero.Flights.Domain.Rules;
using Aero.Flights.Domain.Scheduling;
using Aero.Flights.Domain.SeasonalFlightSchedules;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Flights.Infrastructure.Persistence;

public sealed class AeroFlightDbContext(DbContextOptions<AeroFlightDbContext> options) : DataStoreContext(options), IDataStoreContext
{
    public DbSet<ArrivalFlight> ArrivalFlightSet => Set<ArrivalFlight>();
    public DbSet<DepartureFlight> DepartureFlightSet => Set<DepartureFlight>();
    public DbSet<OpsFlightParkPosition> FlightParkPositionSet => Set<OpsFlightParkPosition>();

    public DbSet<OpsArrivalGate> OpsArrivalGateSet => Set<OpsArrivalGate>();
    public DbSet<OpsDepartureGate> OpsDepartureGateSet => Set<OpsDepartureGate>();
    public DbSet<OpsArrivalBelt> OpsArrivalBeltSet => Set<OpsArrivalBelt>();
    public DbSet<OpsDepartureBelt> OpsDepartureBeltSet => Set<OpsDepartureBelt>();
    public DbSet<OpsCounter> OpsDepartureCounterSet => Set<OpsCounter>();
    public DbSet<OpsArrivalCodeShare> OpsArrivalCodeShareSet => Set<OpsArrivalCodeShare>();
    public DbSet<OpsDepartureCodeShare> OpsDepartureCodeShareSet => Set<OpsDepartureCodeShare>();

    public DbSet<OpsArrivalDelay> ArrivalDelaySet => Set<OpsArrivalDelay>();
    public DbSet<OpsDepartureDelay> DepartureDelaySet => Set<OpsDepartureDelay>();

    public DbSet<OpsArrivalMovementMessage> ArrivalMovementMessageSet => Set<OpsArrivalMovementMessage>();
    public DbSet<OpsDepartureMovementMessage> DepartureMovementMessageSet => Set<OpsDepartureMovementMessage>();

    public DbSet<OpsArrivalService> ArrivalServiceSet => Set<OpsArrivalService>();
    public DbSet<OpsDepartureService> DepartureServiceSet => Set<OpsDepartureService>();

    public DbSet<ArrivalBelt> ArrivalBeltSet => Set<ArrivalBelt>();
    public DbSet<DepartureBelt> DepartureBeltSet => Set<DepartureBelt>();

    public DbSet<OpsArrivalPassenger> ArrivalPassengerSet => Set<OpsArrivalPassenger>();
    public DbSet<OpsDeparturePassenger> DeparturePassengerSet => Set<OpsDeparturePassenger>();
    public DbSet<OpsTransferPassenger> TransferPassengerSet => Set<OpsTransferPassenger>();

    public DbSet<OpsArrivalRouting> ArrivalRoutingSet => Set<OpsArrivalRouting>();
    public DbSet<OpsDepartureRouting> DepartureRoutingSet => Set<OpsDepartureRouting>();

    public DbSet<ScheduleSeason> ScheduleSeasonSet => Set<ScheduleSeason>();
    public DbSet<SeasonalFlightScheduleArrivalFlight> SeasonalFlightScheduleArrivalFlightSet => Set<SeasonalFlightScheduleArrivalFlight>();
    public DbSet<SeasonalFlightScheduleDepartureFlight> SeasonalFlightScheduleDepartureFlightSet => Set<SeasonalFlightScheduleDepartureFlight>();
    public DbSet<SeasonalFlightScheduleArrivalFlightRouting> SeasonalFlightScheduleArrivalFlightRoutingSet => Set<SeasonalFlightScheduleArrivalFlightRouting>();
    public DbSet<SeasonalFlightScheduleDepartureFlightRouting> SeasonalFlightScheduleDepartureFlightRoutingSet => Set<SeasonalFlightScheduleDepartureFlightRouting>();

    public DbSet<Airport> AirportSet => Set<Airport>();
    public DbSet<Airline> AirlineSet => Set<Airline>();
    public DbSet<AirlineContact> AirlineContactSet => Set<AirlineContact>();
    public DbSet<AirlineLogo> AirlineLogoSet => Set<AirlineLogo>();
    public DbSet<AirportImage> AirportImageSet => Set<AirportImage>();
    public DbSet<Company> CompanySet => Set<Company>();
    public DbSet<CompanyType> CompanyTypeSet => Set<CompanyType>();
    public DbSet<CompanyContact> CompanyContactSet => Set<CompanyContact>();
    public DbSet<CheckinClass> CheckinClassSet => Set<CheckinClass>();
    public DbSet<AircraftType> AircraftTypeSet => Set<AircraftType>();
    public DbSet<Aircraft> AircraftSet => Set<Aircraft>();
    public DbSet<Terminal> TerminalSet => Set<Terminal>();
    public DbSet<Gate> GateSet => Set<Gate>();
    public DbSet<Counter> CounterSet => Set<Counter>();
    public DbSet<ParkPosition> ParkingPositionSet => Set<ParkPosition>();
    public DbSet<Runway> RunwaySet => Set<Runway>();
    public DbSet<DelayReason> DelayReasonSet => Set<DelayReason>();
    public DbSet<FlightStatus> FlightStatusSet => Set<FlightStatus>();
    public DbSet<FlightType> FlightTypeSet => Set<FlightType>();
    public DbSet<ArrivalBeltStatus> ArrivalBeltStatusSet => Set<ArrivalBeltStatus>();
    public DbSet<DepartureBeltStatus> DepartureBeltStatusSet => Set<DepartureBeltStatus>();
    public DbSet<GateStatus> GateStatusSet => Set<GateStatus>();
    public DbSet<CounterStatus> CounterStatusSet => Set<CounterStatus>();
    public DbSet<GateType> GateTypeSet => Set<GateType>();
    public DbSet<ParkPositionType> ParkPositionTypeSet => Set<ParkPositionType>();
    public DbSet<CountryType> CountryTypeSet => Set<CountryType>();
    public DbSet<Country> CountrySet => Set<Country>();
    public DbSet<DeBoardingStatus> DeBoardingStatusSet => Set<DeBoardingStatus>();
    public DbSet<Region> RegionSet => Set<Region>();
    public DbSet<HandlingService> HandlingServiceSet => Set<HandlingService>();
    public DbSet<AircraftSizeCategory> AircraftSizeCategorySet => Set<AircraftSizeCategory>();
    public DbSet<Remark> RemarkSet => Set<Remark>();
    public DbSet<AircraftNoiseCategory> AircraftNoiseCategorySet => Set<AircraftNoiseCategory>();
    public DbSet<Timezone> TimezoneSet => Set<Timezone>();
    // Metadata audit logs
    public DbSet<AircraftAudit> AircraftAuditSet => Set<AircraftAudit>();
    public DbSet<AircraftSizeCategoryAudit> AircraftSizeCategoryAuditSet => Set<AircraftSizeCategoryAudit>();
    public DbSet<AircraftTypeAudit> AircraftTypeAuditSet => Set<AircraftTypeAudit>();
    public DbSet<AirlineAudit> AirlineAuditSet => Set<AirlineAudit>();
    public DbSet<AirportAudit> AirportAuditSet => Set<AirportAudit>();
    public DbSet<ArrivalBeltAudit> ArrivalBeltAuditSet => Set<ArrivalBeltAudit>();
    public DbSet<CounterAudit> CounterAuditSet => Set<CounterAudit>();
    public DbSet<CountryAudit> CountryAuditSet => Set<CountryAudit>();
    public DbSet<DelayReasonAudit> DelayReasonAuditSet => Set<DelayReasonAudit>();
    public DbSet<DepartureBeltAudit> DepartureBeltAuditSet => Set<DepartureBeltAudit>();
    public DbSet<GateAudit> GateAuditSet => Set<GateAudit>();

    public DbSet<HandlingServiceAudit> HandlingServiceAuditSet => Set<HandlingServiceAudit>();
    public DbSet<RemarkAudit> RemarkAuditSet => Set<RemarkAudit>();
    public DbSet<RunwayAudit> RunwayAuditSet => Set<RunwayAudit>();
    public DbSet<ParkPositionAudit> ParkPositionAuditSet => Set<ParkPositionAudit>();
    public DbSet<TerminalAudit> TerminalAuditSet => Set<TerminalAudit>();
    public DbSet<CompanyAudit> CompanyAuditSet => Set<CompanyAudit>();
    public DbSet<CompanyTypeAudit> CompanyTypeAuditSet => Set<CompanyTypeAudit>();
    public DbSet<CheckinClassAudit> CheckinClassAuditSet => Set<CheckinClassAudit>();

    public DbSet<AeroConfiguration> AeroConfigurationSet => Set<AeroConfiguration>();
    public DbSet<SystemProperty> SystemPropertySet => Set<SystemProperty>();

    public DbSet<OutboxMessage> OutboxMessageSet => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessageSet => Set<InboxMessage>();

    public DbSet<Rule> RuleSet => Set<Rule>();
    public DbSet<RuleTriggerAssignment> RuleTriggerAssignmentSet => Set<RuleTriggerAssignment>();
    public DbSet<RuleConditionGroup> RuleConditionGroupSet => Set<RuleConditionGroup>();
    public DbSet<RuleCondition> RuleConditionSet => Set<RuleCondition>();
    public DbSet<RuleAction> RuleActionSet => Set<RuleAction>();

    public DbSet<RuleActionTypeMetadata> RuleActionTypeMetadataSet => Set<RuleActionTypeMetadata>();
    public DbSet<RuleTriggerMetadata> RuleTriggerMetadataSet => Set<RuleTriggerMetadata>();
    public DbSet<RuleFieldMetadata> RuleFieldMetadataSet => Set<RuleFieldMetadata>();
    public DbSet<RuleFieldTriggerMetadata> RuleFieldTriggerMetadataSet => Set<RuleFieldTriggerMetadata>();
    public DbSet<RuleFieldActionTypeMetadata> RuleFieldActionTypeMetadataSet => Set<RuleFieldActionTypeMetadata>();

    public DbSet<JobSchedule> JobScheduleSet => Set<JobSchedule>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        // TPC for all abstract hierarchies — must be set before ApplyConfigurationsFromAssembly
        modelBuilder.Entity<Flight>().UseTpcMappingStrategy();
        modelBuilder.Entity<ResourceBase>().UseTpcMappingStrategy();
        modelBuilder.Entity<Delay>().UseTpcMappingStrategy();
        modelBuilder.Entity<MovementMessage>().UseTpcMappingStrategy();
        modelBuilder.Entity<OpsService>().UseTpcMappingStrategy();
        modelBuilder.Entity<OpsPassenger>().UseTpcMappingStrategy();
        modelBuilder.Entity<OpsRouting>().UseTpcMappingStrategy();
        modelBuilder.Entity<OpsCodeShare>().UseTpcMappingStrategy();
        modelBuilder.Entity<SeasonalFlightScheduleFlight>().UseTpcMappingStrategy();
        modelBuilder.Entity<SeasonalFlightScheduleRouting>().UseTpcMappingStrategy();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AeroFlightDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

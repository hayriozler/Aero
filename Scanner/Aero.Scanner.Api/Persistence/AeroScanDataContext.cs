using Aero.Scanner.Domain.Aggregates.Bags;
using Aero.Scanner.Domain.Aggregates.Companies;
using Aero.Scanner.Domain.Aggregates.Containers;
using Aero.Scanner.Domain.Aggregates.Flights;
using Aero.Scanner.Domain.Aggregates.HandlingContracts;
using Aero.Scanner.Domain.Aggregates.HHTs;
using Aero.Scanner.Domain.Aggregates.Mappings;
using Aero.Scanner.Domain.Aggregates.Reconciliations;
using Aero.Scanner.Domain.Aggregates.Settings;
using Aero.Scanner.Domain.Aggregates.Users;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aero.Scanner.Api.Persistence;


public class AeroScanDataContext(DbContextOptions<AeroScanDataContext> options)
    : DataStoreContext(options)
{

    // Baggages
    public DbSet<ArrivalBag> ArrivalBagSet => Set<ArrivalBag>();
    public DbSet<ArrivalBagEvent> ArrivalBagEventSet => Set<ArrivalBagEvent>();
    public DbSet<ArrivalFlightReconciliation> ArrivalFlightReconciliationSet => Set<ArrivalFlightReconciliation>();
    public DbSet<ArrivalFlightPassenger> ArrivalFlightPassengerSet => Set<ArrivalFlightPassenger>();
    public DbSet<DepartureBag> DepartureBagSet => Set<DepartureBag>();
    public DbSet<DepartureBagEvent> DepartureBagEventSet => Set<DepartureBagEvent>();
    public DbSet<DepartureFlightReconciliation> DepartureFlightReconciliationSet => Set<DepartureFlightReconciliation>();
    public DbSet<DepartureFlightPassenger> DepartureFlightPassengerSet => Set<DepartureFlightPassenger>();

    // Flights
    public DbSet<DepartureFlight> DepartureFlightSet => Set<DepartureFlight>();
    public DbSet<DepartureFlightLog> DepartureFlightLogSet => Set<DepartureFlightLog>();

    public DbSet<ArrivalFlight> ArrivalFlightSet => Set<ArrivalFlight>();
    public DbSet<ArrivalFlightLog> ArrivalFlightLogSet => Set<ArrivalFlightLog>();

    public DbSet<ReconciliationRecord> ReconciliationRecordSet => Set<ReconciliationRecord>();
    public DbSet<Container> ContainerSet => Set<Container>();


    // Reference data

    public DbSet<AirlineHandlingContract> HandlingContractSet => Set<AirlineHandlingContract>();
    public DbSet<AirlineHandlingContractFlightNumber> HandlingContractFlightSet => Set<AirlineHandlingContractFlightNumber>();
    public DbSet<Company> CompanySet => Set<Company>();
    public DbSet<Role> RoleSet => Set<Role>();
    public DbSet<Permission> PermissionSet => Set<Permission>();
    public DbSet<ScannerUser> UserSet => Set<ScannerUser>();
    public DbSet<HandheldTerminal> HandheldTerminalSet => Set<HandheldTerminal>();
    public DbSet<ContainerType> ContainerTypeSet => Set<ContainerType>();
    public DbSet<ContainerTypeClass> ContainerTypeClassSet => Set<ContainerTypeClass>();

    public DbSet<AirlineClassMap> AirlineClassMapSet => Set<AirlineClassMap>();
    public DbSet<ResourceStatusMap> ResourceStatusMapSet => Set<ResourceStatusMap>();
    public DbSet<SystemConfiguration> SystemConfigurationSet => Set<SystemConfiguration>();


    // Audit logs
    public DbSet<AirlineClassMapLog> AirlineClassLogMapSet => Set<AirlineClassMapLog>();
    public DbSet<AirlineHandlingContractFlightNumberLog> AirlineHandlingContractFlightNumberLogSet => Set<AirlineHandlingContractFlightNumberLog>();
    public DbSet<AirlineHandlingContractLog> AirlineHandlingContractLogSet => Set<AirlineHandlingContractLog>();

    public DbSet<CompanyLog> CompanyLogSet => Set<CompanyLog>();
    public DbSet<ContainerLog> ContainerLogSet => Set<ContainerLog>();
    public DbSet<ContainerTypeLog> ContainerTypeLogSet => Set<ContainerTypeLog>();
    public DbSet<ContainerTypeClassLog> ContainerTypeClassLogSet => Set<ContainerTypeClassLog>();

    public DbSet<HandheldTerminalLog> HandheldTerminalLogSet => Set<HandheldTerminalLog>();
    public DbSet<PermissionLog> PermissionLogSet => Set<PermissionLog>();
    public DbSet<ResourceStatusMapLog> ResourceStatusMapLogSet => Set<ResourceStatusMapLog>();
    public DbSet<RoleLog> RoleLogSet => Set<RoleLog>();
    public DbSet<UserLog> UserLogSet => Set<UserLog>();
    public DbSet<SystemConfigurationLog> SystemConfigurationLogSet => Set<SystemConfigurationLog>();
}

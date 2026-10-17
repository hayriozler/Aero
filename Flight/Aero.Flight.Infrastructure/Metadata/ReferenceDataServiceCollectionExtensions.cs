using Aero.Flights.Application.Common;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Application.Tenancy;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Common;
using Aero.Flights.Domain.Metadatas;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.Flights.Infrastructure.Metadata;

public static class MetadataDataServiceCollectionExtensions
{
    public static IServiceCollection AddMetadataDataEntities(this IServiceCollection services)
    {
        services.AddMetadataCrud<Gate, GateRequest, GateResponse, GateMetadataMap, GateAudit>();
        services.AddMetadataCrud<Airport, AirportRequest, AirportResponse, AirportMetadataMap, AirportAudit>();
        services.AddMetadataCrud<Airline, AirlineRequest, AirlineResponse, AirlineMetadataMap, AirlineAudit>();
        services.AddMetadataCrud<Company, CompanyRequest, CompanyResponse, CompanyMetadataMap, CompanyAudit>();
        services.AddMetadataCrud<CompanyType, CompanyTypeRequest, CompanyTypeResponse, CompanyTypeMetadataMap, CompanyTypeAudit>();
        services.AddMetadataCrud<CheckinClass, CheckinClassRequest, CheckinClassResponse, CheckinClassMetadataMap, CheckinClassAudit>();
        services.AddMetadataCrud<ArrivalBelt, ArrivalBeltRequest, ArrivalBeltResponse, ArrivalBeltMetadataMap, ArrivalBeltAudit>();
        services.AddMetadataCrud<Counter, CounterRequest, CounterResponse, CounterMetadataMap, CounterAudit>();
        services.AddMetadataCrud<Country, CountryRequest, CountryResponse, CountryMetadataMap, CountryAudit>();
        services.AddMetadataCrud<DelayReason, DelayReasonRequest, DelayReasonResponse, DelayReasonMetadataMap, DelayReasonAudit>();
        services.AddMetadataCrud<DepartureBelt, DepartureBeltRequest, DepartureBeltResponse, DepartureBeltMetadataMap, DepartureBeltAudit>();
        services.AddMetadataCrud<Remark, RemarkRequest, RemarkResponse, RemarkMetadataMap, RemarkAudit>();
        services.AddMetadataCrud<Runway, RunwayRequest, RunwayResponse, RunwayMetadataMap, RunwayAudit>();
        services.AddMetadataCrud<ParkPosition, ParkPositionRequest, ParkPositionResponse, ParkPositionMetadataMap, ParkPositionAudit>();
        services.AddMetadataCrud<HandlingService, HandlingServiceRequest, HandlingServiceResponse, HandlingServiceMetadataMap, HandlingServiceAudit>();
        services.AddMetadataCrud<Terminal, TerminalRequest, TerminalResponse, TerminalMetadataMap, TerminalAudit>();
        services.AddMetadataCrud<Aircraft, AircraftRequest, AircraftResponse, AircraftMetadataMap, AircraftAudit>();
        services.AddMetadataCrud<AircraftType, AircraftTypeRequest, AircraftTypeResponse, AircraftTypeMetadataMap, AircraftTypeAudit>();
        services.AddMetadataCrud<AircraftSizeCategory, AircraftSizeCategoryRequest, AircraftSizeCategoryResponse, AircraftSizeCategoryMetadataMap, AircraftSizeCategoryAudit>();

        // Pure lookups have no Create/Update/Delete handlers — nothing in the app can ever change
        // them, so it's safe to cache List/Get results for a long time per tenant. CRUD Metadata
        // entities (above) are also cached via AddMetadataLookup inside AddMetadataCrud, just with
        // a much shorter window (MetadataCacheDuration.Crud) since they can be edited at any time.
        services.AddCachedMetadataLookup<ArrivalBeltStatus, ArrivalBeltStatusResponse, ArrivalBeltStatusReader>();
        services.AddCachedMetadataLookup<DepartureBeltStatus, DepartureBeltStatusResponse, DepartureBeltStatusReader>();
        services.AddCachedMetadataLookup<CounterStatus, CounterStatusResponse, CounterStatusReader>();
        services.AddCachedMetadataLookup<CountryType, CountryTypeResponse, CountryTypeReader>();
        services.AddCachedMetadataLookup<DeBoardingStatus, DeBoardingStatusResponse, DeBoardingStatusReader>();
        services.AddCachedMetadataLookup<FlightStatus, FlightStatusResponse, FlightStatusReader>();
        services.AddCachedMetadataLookup<FlightType, FlightTypeResponse, FlightTypeReader>();
        services.AddCachedMetadataLookup<GateStatus, GateStatusResponse, GateStatusReader>();
        services.AddCachedMetadataLookup<GateType, GateTypeResponse, GateTypeReader>();
        services.AddCachedMetadataLookup<ParkPositionType, ParkPositionTypeResponse, ParkPositionTypeReader>();
        services.AddCachedMetadataLookup<Region, RegionResponse, RegionReader>();
        services.AddCachedMetadataLookup<AircraftNoiseCategory, AircraftNoiseCategoryResponse, AircraftNoiseCategoryReader>();
        services.AddCachedMetadataLookup<Timezone, TimezoneResponse, TimezoneReader>();

        services.AddScoped<IInboundMessageProcessor, MetadataDataInboundProcessor>();

        // Delete guards — only entities actually reachable from flight/business data need one;
        // everything else in AddMetadataCrud above stays guard-free (MetadataDeleteCommandHandler
        // treats a missing guard as "nothing to check", same as before this existed).
        services.AddScoped<IMetadataUsageGuard<Airline>, AirlineUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<FlightType>, FlightTypeUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<AircraftType>, AircraftTypeUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Airport>, AirportUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Aircraft>, AircraftUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<AircraftSizeCategory>, AircraftSizeCategoryUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<ParkPosition>, ParkPositionUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Terminal>, TerminalUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Country>, CountryUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<CompanyType>, CompanyTypeUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Gate>, GateUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Counter>, CounterUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<HandlingService>, HandlingServiceUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<ArrivalBelt>, ArrivalBeltUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<DepartureBelt>, DepartureBeltUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<CheckinClass>, CheckinClassUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<DelayReason>, DelayReasonUsageGuard>();
        services.AddScoped<IMetadataUsageGuard<Company>, CompanyUsageGuard>();

        return services;
    }

    // For CRUD-capable Metadata entities. List/Get are cached briefly (MetadataCacheDuration.Crud)
    // via CachedCrudListQueryHandler/CachedMetadataGetQueryHandler — both stay on the tracked
    // MetadataListQueryHandler/MetadataGetQueryHandler underneath, just wrapped in a short cache
    // window instead of hitting the database on every call.
    public static IServiceCollection AddMetadataLookup<TEntity, TResponse, TReader>(this IServiceCollection services)
        where TEntity : class
        where TReader : class, IMetadataReader<TEntity, TResponse>
    {
        services.AddSingleton<IMetadataReader<TEntity, TResponse>, TReader>();
        services.AddScoped<MetadataListQueryHandler<TEntity, TResponse>>();
        services.AddScoped<MetadataGetQueryHandler<TEntity, TResponse>>();

        services.AddScoped<IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>>>(sp =>
            new CachedCrudListQueryHandler<TEntity, TResponse>(
                sp.GetRequiredService<MetadataListQueryHandler<TEntity, TResponse>>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ITenantContext>()));

        services.AddScoped<IQueryHandler<MetadataGetQuery<TEntity>, TResponse?>>(sp =>
            new CachedMetadataGetQueryHandler<TEntity, TResponse>(
                sp.GetRequiredService<MetadataGetQueryHandler<TEntity, TResponse>>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ITenantContext>(),
                MetadataCacheDuration.Crud));

        return services;
    }

    // For entities with no Create/Update/Delete handlers — List/Get are cached for a long time
    // (MetadataCacheDuration.Lookup) since nothing in the app can ever change them.
    public static IServiceCollection AddCachedMetadataLookup<TEntity, TResponse, TReader>(this IServiceCollection services)
        where TEntity : class
        where TReader : class, IMetadataReader<TEntity, TResponse>
    {
        services.AddSingleton<IMetadataReader<TEntity, TResponse>, TReader>();
        services.AddScoped<MetadataGetQueryHandler<TEntity, TResponse>>();
        services.AddScoped<IQueryHandler<MetadataListQuery<TEntity>, IReadOnlyList<TResponse>>, CachedMetadataListQueryHandler<TEntity, TResponse>>();

        services.AddScoped<IQueryHandler<MetadataGetQuery<TEntity>, TResponse?>>(sp =>
            new CachedMetadataGetQueryHandler<TEntity, TResponse>(
                sp.GetRequiredService<MetadataGetQueryHandler<TEntity, TResponse>>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ITenantContext>(),
                MetadataCacheDuration.Lookup));

        return services;
    }

    public static IServiceCollection AddMetadataCrud<TEntity, TRequest, TResponse, TMap, TLog>(this IServiceCollection services)
        where TEntity : class
        where TMap : class, IMetadataMap<TEntity, TRequest, TResponse>
        where TLog : AuditLogBase
    {
        services.AddMetadataLookup<TEntity, TResponse, TMap>();
        services.AddSingleton<IMetadataMap<TEntity, TRequest, TResponse>, TMap>();

        // Wrapped in Cached*CommandHandlers so a Create/Update/Delete evicts the list/get cache
        // entries immediately instead of leaving the change invisible until the cache window expires.
        services.AddScoped<MetadataCreateCommandHandler<TEntity, TRequest, TResponse>>();
        services.AddScoped<ICommandHandler<MetadataCreateCommand<TEntity, TRequest>, TResponse>, CachedMetadataCreateCommandHandler<TEntity, TRequest, TResponse>>();
        services.AddScoped<MetadataUpdateCommandHandler<TEntity, TRequest, TResponse>>();
        services.AddScoped<ICommandHandler<MetadataUpdateCommand<TEntity, TRequest>, TResponse?>, CachedMetadataUpdateCommandHandler<TEntity, TRequest, TResponse>>();
        services.AddScoped<MetadataDeleteCommandHandler<TEntity>>();
        services.AddScoped<ICommandHandler<MetadataDeleteCommand<TEntity>, MetadataDeleteResult>, CachedMetadataDeleteCommandHandler<TEntity>>();
        services.AddScoped<IMetadataInboundApplier, MetadataInboundApplier<TEntity, TRequest, TResponse>>();
        services.AddSingleton<IMetadataOutboxMapper, MetadataOutboxMapper<TEntity, TRequest, TResponse>>();
        services.AddScoped<IQueryHandler<MetadataAuditQuery<TEntity>, IReadOnlyList<AuditEntryResponse>>, MetadataAuditQueryHandler<TEntity, TLog>>();
        return services;
    }
}

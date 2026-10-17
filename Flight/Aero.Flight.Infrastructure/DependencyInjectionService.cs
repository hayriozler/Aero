using Aero.Flights.Application.Common;
using Aero.Flights.Application.Configuration;
using Aero.Flights.Application.Flights;
using Aero.Flights.Application.Flights.CodeShares;
using Aero.Flights.Application.Flights.Commands;
using Aero.Flights.Application.Flights.Delays;
using Aero.Flights.Application.Flights.InboundProcessors;
using Aero.Flights.Application.Flights.MovementMessages;
using Aero.Flights.Application.Flights.ParkPositiions;
using Aero.Flights.Application.Flights.Queries;
using Aero.Flights.Application.Flights.Resources;
using Aero.Flights.Application.Flights.Routings;
using Aero.Flights.Application.Flights.Services;
using Aero.Flights.Application.Identity;
using Aero.Flights.Application.Identity.Auth;
using Aero.Flights.Application.Identity.Roles;
using Aero.Flights.Application.Identity.Users;
using Aero.Flights.Application.Interfaces;
using Aero.Flights.Application.Metadatas;
using Aero.Flights.Application.Resources;
using Aero.Flights.Application.SeasonalFlightSchedules;
using Aero.Flights.Contracts.Flights;
using Aero.Flights.Contracts.Identity;
using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Contracts.SeasonalFlightSchedules;
using Aero.Flights.Domain.Audit.Identity;
using Aero.Flights.Domain.Identity;
using Aero.Flights.Domain.Ops;
using Aero.Flights.Domain.Ops.Delays;
using Aero.Flights.Domain.Ops.MovementMessages;
using Aero.Flights.Domain.Ops.Resources;
using Aero.Flights.Domain.Ops.Routing;
using Aero.Flights.Domain.Ops.Services;
using Aero.Flights.Infrastructure.Identity;
using Aero.Flights.Infrastructure.Messaging;
using Aero.Flights.Infrastructure.Metadata;
using Aero.Flights.Infrastructure.Persistence;
using Aero.Flights.Infrastructure.Persistence.Interceptors;
using Aero.Flights.Infrastructure.Rules;
using Aero.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.Flights.Infrastructure;

public static class DependencyInjectionService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<MetadataOutboxInterceptor>();
        services.AddScoped<FlightOutboxInterceptor>();

        // Connection comes from the tenant resolved for the current scope (middleware or background loop).
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddDbContext<AeroFlightDbContext>((sp, options) =>
        {
            var audit = sp.GetRequiredService<AuditInterceptor>();
            var outbox = sp.GetRequiredService<MetadataOutboxInterceptor>();
            var flightOutbox = sp.GetRequiredService<FlightOutboxInterceptor>();
            var tenant = sp.GetRequiredService<ITenantContext>();

            options.UseNpgsql(tenant.ConnectionString,
                npgsql => npgsql.MigrationsAssembly("AeroFlight.Migrations")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .AddInterceptors(audit, outbox, flightOutbox);

#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IAeroFlightDbContext>(sp => sp.GetRequiredService<AeroFlightDbContext>());

        services.AddIdentityHandlers();
        services.AddMetadataDataEntities();
        services.AddFlightCoreHandlers();
        services.AddFlightSubResourceCrud();
        services.AddReferenceDataHandlers();
        services.AddSeasonalScheduleHandlers();
        services.AddFlightResourceHandlers();
        services.AddRules();
        services.AddFlightInboundProcessors();
        services.AddMessaging();

        return services;
    }

    private static IServiceCollection AddIdentityHandlers(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasherService, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();

        services.AddScoped<ICommandHandler<LoginCommand, LoginResponse?>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<ClientTokenCommand, ClientTokenResponse?>, ClientTokenCommandHandler>();

        services.AddScoped<IQueryHandler<GetUsersQuery, IReadOnlyList<UserResponse>>, GetUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserQuery, UserResponse?>, GetUserQueryHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand, UserResponse>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UserResponse?>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRoleCommand>, AssignRoleCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveRoleCommand>, RemoveRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, DeleteUserResult>, DeleteUserCommandHandler>();

        services.AddScoped<IQueryHandler<GetRolesQuery, IReadOnlyList<RoleResponse>>, GetRolesQueryHandler>();
        services.AddScoped<IQueryHandler<GetRoleQuery, RoleResponse?>, GetRoleQueryHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, RoleResponse>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRoleCommand, RoleResponse?>, UpdateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand, DeleteRoleResult>, DeleteRoleCommandHandler>();
        services.AddScoped<IQueryHandler<GetRolePermissionsQuery, IReadOnlyList<string>>, GetRolePermissionsQueryHandler>();
        services.AddScoped<ICommandHandler<SetRolePermissionsCommand>, SetRolePermissionsCommandHandler>();

        services.AddScoped<IQueryHandler<MetadataAuditQuery<User>, IReadOnlyList<AuditEntryResponse>>, IdentityAuditQueryHandler<User, UserAudit>>();
        services.AddScoped<IQueryHandler<MetadataAuditQuery<Role>, IReadOnlyList<AuditEntryResponse>>, IdentityAuditQueryHandler<Role, RoleAudit>>();
        return services;
    }

    private static IServiceCollection AddFlightCoreHandlers(this IServiceCollection services)
    {
        services.AddScoped<IFlightCountryTypeResolver, FlightCountryTypeResolver>();
        services.AddScoped<IFlightAutoLinker, FlightAutoLinker>();
        services.AddScoped<IFlightCreationService, FlightCreationService>();
        services.AddScoped<IFlightUpdateService, FlightUpdateService>();
        services.AddScoped<IQueryHandler<GetFlightsQuery, PagedResult<MergedFlightRow>>, GetFlightsQueryHandler>();
        services.AddScoped<ICommandHandler<CreateArrivalFlightCommand, CreateFlightResult>, CreateArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<CreateDepartureFlightCommand, CreateFlightResult>, CreateDepartureFlightCommandHandler>();
        services.AddScoped<IQueryHandler<GetArrivalFlightDetailQuery, ArrivalFlightDetail?>, GetArrivalFlightDetailQueryHandler>();
        services.AddScoped<IQueryHandler<GetDepartureFlightDetailQuery, DepartureFlightDetail?>, GetDepartureFlightDetailQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateArrivalFlightCommand, UpdateFlightResult>, UpdateArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateDepartureFlightCommand, UpdateFlightResult>, UpdateDepartureFlightCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteArrivalFlightCommand, bool>, DeleteArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteDepartureFlightCommand, bool>, DeleteDepartureFlightCommandHandler>();
        services.AddScoped<ICommandHandler<UnlinkArrivalFlightCommand, bool>, UnlinkArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<UnlinkDepartureFlightCommand, bool>, UnlinkDepartureFlightCommandHandler>();
        services.AddScoped<ICommandHandler<LinkFlightsCommand, LinkFlightsResult>, LinkFlightsCommandHandler>();
        services.AddScoped<IQueryHandler<GetGanttItemsQuery, IReadOnlyList<GanttItem>>, GetGanttItemsQueryHandler>();
        services.AddScoped<IQueryHandler<GetUnassignedFlightsQuery, IReadOnlyList<UnassignedFlightItem>>, GetUnassignedFlightsQueryHandler>();
        services.AddScoped<IQueryHandler<GetFlightFullAuditQuery, IReadOnlyList<AuditEntryResponse>>, GetFlightFullAuditQueryHandler>();
        services.AddScoped<IQueryHandler<GetOpsOverviewQuery, OpsOverview>, GetOpsOverviewQueryHandler>();

        services.AddFieldUpdateCrud<ArrivalFlight>();
        services.AddFieldUpdateCrud<DepartureFlight>();

        services.AddScoped<IAeroConfigService, AeroConfigService>();
        return services;
    }

    private static IServiceCollection AddFlightSubResourceCrud(this IServiceCollection services)
    {
        services.AddRoutingCrud<OpsArrivalRouting, ArrivalRoutingFactory, ArrivalRoutingCountryTypeUpdater>();
        services.AddRoutingCrud<OpsDepartureRouting, DepartureRoutingFactory, DepartureRoutingCountryTypeUpdater>();

        services.AddCodeShareCrud<OpsArrivalCodeShare, ArrivalCodeShareFactory>();
        services.AddCodeShareCrud<OpsDepartureCodeShare, DepartureCodeShareFactory>();

        services.AddDelayCrud<OpsArrivalDelay, ArrivalDelayFactory>();
        services.AddDelayCrud<OpsDepartureDelay, DepartureDelayFactory>();

        services.AddMovementMessageQuery<OpsArrivalMovementMessage, ArrivalMovementMessageFactory>();
        services.AddMovementMessageQuery<OpsDepartureMovementMessage, DepartureMovementMessageFactory>();

        services.AddServiceCrud<OpsArrivalService, ArrivalServiceFactory>();
        services.AddServiceCrud<OpsDepartureService, DepartureServiceFactory>();
        return services;
    }

    private static IServiceCollection AddReferenceDataHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<ListAirlineContactsQuery, IReadOnlyList<AirlineContactResponse>>, ListAirlineContactsQueryHandler>();
        services.AddScoped<ICommandHandler<AddAirlineContactCommand, AirlineContactResponse>, AddAirlineContactCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateAirlineContactCommand, AirlineContactResponse?>, UpdateAirlineContactCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAirlineContactCommand, bool>, DeleteAirlineContactCommandHandler>();

        services.AddScoped<IQueryHandler<ListAirlineLogosQuery, IReadOnlyList<AirlineLogoResponse>>, ListAirlineLogosQueryHandler>();
        services.AddScoped<IQueryHandler<GetAirlineLogoImageQuery, AirlineLogoImage?>, GetAirlineLogoImageQueryHandler>();
        services.AddScoped<ICommandHandler<AddAirlineLogoCommand, AirlineLogoResponse>, AddAirlineLogoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateAirlineLogoNameCommand, AirlineLogoResponse?>, UpdateAirlineLogoNameCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAirlineLogoCommand, bool>, DeleteAirlineLogoCommandHandler>();

        services.AddScoped<IQueryHandler<ListAirportImagesQuery, IReadOnlyList<AirportImageResponse>>, ListAirportImagesQueryHandler>();
        services.AddScoped<IQueryHandler<GetAirportImageQuery, AirportImageData?>, GetAirportImageQueryHandler>();
        services.AddScoped<ICommandHandler<AddAirportImageCommand, AirportImageResponse>, AddAirportImageCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAirportImageCommand, bool>, DeleteAirportImageCommandHandler>();

        services.AddScoped<IQueryHandler<ListCompanyContactsQuery, IReadOnlyList<CompanyContactResponse>>, ListCompanyContactsQueryHandler>();
        services.AddScoped<ICommandHandler<AddCompanyContactCommand, CompanyContactResponse>, AddCompanyContactCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCompanyContactCommand, CompanyContactResponse?>, UpdateCompanyContactCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCompanyContactCommand, bool>, DeleteCompanyContactCommandHandler>();

        services.AddScoped<IQueryHandler<ListParkPositionsQuery, IReadOnlyList<OpsParkPositionResponse>>, ListParkPositionsQueryHandler>();
        services.AddScoped<ICommandHandler<AddParkPositionCommand, OpsParkPositionResponse>, AddParkPositionCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateParkPositionCommand, OpsParkPositionResponse?>, UpdateParkPositionCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteParkPositionCommand, bool>, DeleteParkPositionCommandHandler>();
        return services;
    }

    private static IServiceCollection AddSeasonalScheduleHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<ListScheduleSeasonsQuery, IReadOnlyList<ScheduleSeasonResponse>>, ListScheduleSeasonsQueryHandler>();
        services.AddScoped<IQueryHandler<GetScheduleSeasonQuery, ScheduleSeasonResponse?>, GetScheduleSeasonQueryHandler>();
        services.AddScoped<ICommandHandler<CreateScheduleSeasonCommand, ScheduleSeasonResponse>, CreateScheduleSeasonCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateScheduleSeasonCommand, ScheduleSeasonResponse?>, UpdateScheduleSeasonCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteScheduleSeasonCommand, bool>, DeleteScheduleSeasonCommandHandler>();

        services.AddScoped<IQueryHandler<ListSeasonalFlightScheduleArrivalFlightsQuery, IReadOnlyList<SeasonalFlightScheduleArrivalFlightResponse>>, ListSeasonalFlightScheduleArrivalFlightsQueryHandler>();
        services.AddScoped<IQueryHandler<GetSeasonalFlightScheduleArrivalFlightQuery, SeasonalFlightScheduleArrivalFlightResponse?>, GetSeasonalFlightScheduleArrivalFlightQueryHandler>();
        services.AddScoped<ICommandHandler<CreateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse>, CreateSeasonalFlightScheduleArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSeasonalFlightScheduleArrivalFlightCommand, SeasonalFlightScheduleArrivalFlightResponse?>, UpdateSeasonalFlightScheduleArrivalFlightCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteSeasonalFlightScheduleArrivalFlightCommand, bool>, DeleteSeasonalFlightScheduleArrivalFlightCommandHandler>();

        services.AddScoped<IQueryHandler<ListSeasonalFlightScheduleDepartureFlightsQuery, IReadOnlyList<SeasonalFlightScheduleDepartureFlightResponse>>, ListSeasonalFlightScheduleDepartureFlightsQueryHandler>();
        services.AddScoped<IQueryHandler<GetSeasonalFlightScheduleDepartureFlightQuery, SeasonalFlightScheduleDepartureFlightResponse?>, GetSeasonalFlightScheduleDepartureFlightQueryHandler>();
        services.AddScoped<ICommandHandler<CreateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse>, CreateSeasonalFlightScheduleDepartureFlightCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSeasonalFlightScheduleDepartureFlightCommand, SeasonalFlightScheduleDepartureFlightResponse?>, UpdateSeasonalFlightScheduleDepartureFlightCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteSeasonalFlightScheduleDepartureFlightCommand, bool>, DeleteSeasonalFlightScheduleDepartureFlightCommandHandler>();
        return services;
    }

    private static IServiceCollection AddFlightResourceHandlers(this IServiceCollection services)
    {
        services.AddScoped<IResourceDefaultStatusResolver, ResourceDefaultStatusResolver>();
        services.AddScoped<IResourceAutoCloseService, ResourceAutoCloseService>();

        services.AddFlightResourceCrud<OpsArrivalGate, ArrivalGateResourceFactory>();
        services.AddFlightResourceCrud<OpsDepartureGate, DepartureGateResourceFactory>();
        services.AddFlightResourceCrud<OpsArrivalBelt, ArrivalBeltResourceFactory>();
        services.AddFlightResourceCrud<OpsDepartureBelt, DepartureBeltResourceFactory>();
        services.AddFlightResourceCrud<OpsCounter, DepartureCounterResourceFactory>();
        return services;
    }

    private static IServiceCollection AddFlightInboundProcessors(this IServiceCollection services)
    {
        services.AddScoped<IInboundMessageProcessor, ArrivalFlightCreatedProcessor>();
        services.AddScoped<IInboundMessageProcessor, DepartureFlightCreatedProcessor>();
        services.AddScoped<IInboundMessageProcessor, ArrivalFlightUpdatedProcessor>();
        services.AddScoped<IInboundMessageProcessor, DepartureFlightUpdatedProcessor>();
        services.AddScoped<IInboundMessageProcessor, ArrivalFlightDeletedProcessor>();
        services.AddScoped<IInboundMessageProcessor, DepartureFlightDeletedProcessor>();
        services.AddScoped<IInboundMessageProcessor, ArrivalResourceProcessor>();
        services.AddScoped<IInboundMessageProcessor, DepartureResourceProcessor>();
        services.AddScoped<IInboundMessageProcessor, MovementMessageReceivedProcessor>();
        return services;
    }

    // Outbox/inbox draining + cleanup are Quartz jobs — see AddScheduledJobs.
    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        services.AddHostedService<InboxConsumerService>();

#if DEBUG
        services.AddHostedService<DebugMessageSinkService>();
#endif
        return services;
    }

    private static IServiceCollection AddFlightResourceCrud<TResource, TFactory>(this IServiceCollection services)
        where TResource : ResourceBase
        where TFactory : class, IFlightResourceFactory<TResource>
    {
        services.AddSingleton<IFlightResourceFactory<TResource>, TFactory>();
        services.AddScoped<IFlightResourceService<TResource>, FlightResourceService<TResource>>();
        services.AddScoped<IQueryHandler<ListFlightResourcesQuery<TResource>, IReadOnlyList<FlightResourceResponse>>, ListFlightResourcesQueryHandler<TResource>>();
        services.AddScoped<ICommandHandler<AddFlightResourceCommand<TResource>, FlightResourceResponse>, AddFlightResourceCommandHandler<TResource>>();
        services.AddScoped<ICommandHandler<UpdateFlightResourceCommand<TResource>, FlightResourceResponse?>, UpdateFlightResourceCommandHandler<TResource>>();
        services.AddScoped<ICommandHandler<DeleteFlightResourceCommand<TResource>, bool>, DeleteFlightResourceCommandHandler<TResource>>();
        services.AddScoped<ICommandHandler<BulkUpdateResourceStatusCommand<TResource>, IReadOnlyList<FlightResourceResponse>>, BulkUpdateResourceStatusCommandHandler<TResource>>();
        return services;
    }

    private static IServiceCollection AddRoutingCrud<TRouting, TFactory, TCountryTypeUpdater>(this IServiceCollection services)
        where TRouting : OpsRouting
        where TFactory : class, IRoutingFactory<TRouting>
        where TCountryTypeUpdater : class, IRoutingCountryTypeUpdater<TRouting>
    {
        services.AddSingleton<IRoutingFactory<TRouting>, TFactory>();
        services.AddScoped<IRoutingCountryTypeUpdater<TRouting>, TCountryTypeUpdater>();
        services.AddScoped<IQueryHandler<ListRoutingsQuery<TRouting>, IReadOnlyList<OpsRoutingResponse>>, ListRoutingsQueryHandler<TRouting>>();
        services.AddScoped<ICommandHandler<AddRoutingCommand<TRouting>, AddRoutingResult>, AddRoutingCommandHandler<TRouting>>();
        services.AddScoped<ICommandHandler<UpdateRoutingCommand<TRouting>, OpsRoutingResponse?>, UpdateRoutingCommandHandler<TRouting>>();
        services.AddScoped<ICommandHandler<DeleteRoutingCommand<TRouting>, DeleteRoutingOutcome>, DeleteRoutingCommandHandler<TRouting>>();
        return services;
    }

    private static IServiceCollection AddCodeShareCrud<TCodeShare, TFactory>(this IServiceCollection services)
        where TCodeShare : OpsCodeShare
        where TFactory : class, ICodeShareFactory<TCodeShare>
    {
        services.AddSingleton<ICodeShareFactory<TCodeShare>, TFactory>();
        services.AddScoped<IQueryHandler<ListCodeSharesQuery<TCodeShare>, IReadOnlyList<CodeShareResponse>>, ListCodeSharesQueryHandler<TCodeShare>>();
        services.AddScoped<ICommandHandler<AddCodeShareCommand<TCodeShare>, CodeShareResponse>, AddCodeShareCommandHandler<TCodeShare>>();
        services.AddScoped<ICommandHandler<UpdateCodeShareCommand<TCodeShare>, CodeShareResponse?>, UpdateCodeShareCommandHandler<TCodeShare>>();
        services.AddScoped<ICommandHandler<DeleteCodeShareCommand<TCodeShare>, bool>, DeleteCodeShareCommandHandler<TCodeShare>>();
        return services;
    }

    private static IServiceCollection AddDelayCrud<TDelay, TFactory>(this IServiceCollection services)
        where TDelay : Delay
        where TFactory : class, IDelayFactory<TDelay>
    {
        services.AddSingleton<IDelayFactory<TDelay>, TFactory>();
        services.AddScoped<IQueryHandler<ListDelaysQuery<TDelay>, IReadOnlyList<DelayResponse>>, ListDelaysQueryHandler<TDelay>>();
        services.AddScoped<ICommandHandler<AddDelayCommand<TDelay>, DelayResponse>, AddDelayCommandHandler<TDelay>>();
        services.AddScoped<ICommandHandler<UpdateDelayCommand<TDelay>, DelayResponse?>, UpdateDelayCommandHandler<TDelay>>();
        services.AddScoped<ICommandHandler<DeleteDelayCommand<TDelay>, bool>, DeleteDelayCommandHandler<TDelay>>();
        return services;
    }

    private static IServiceCollection AddMovementMessageQuery<TMessage, TFactory>(this IServiceCollection services)
        where TMessage : MovementMessage
        where TFactory : class, IMovementMessageFactory<TMessage>
    {
        services.AddSingleton<IMovementMessageFactory<TMessage>, TFactory>();
        services.AddScoped<IQueryHandler<ListMovementMessagesQuery<TMessage>, IReadOnlyList<MovementMessageResponse>>, ListMovementMessagesQueryHandler<TMessage>>();
        return services;
    }

    private static IServiceCollection AddServiceCrud<TService, TFactory>(this IServiceCollection services)
        where TService : OpsService
        where TFactory : class, IServiceFactory<TService>
    {
        services.AddSingleton<IServiceFactory<TService>, TFactory>();
        services.AddScoped<IQueryHandler<ListServicesQuery<TService>, IReadOnlyList<ServiceResponse>>, ListServicesQueryHandler<TService>>();
        services.AddScoped<ICommandHandler<AddServiceCommand<TService>, ServiceResponse>, AddServiceCommandHandler<TService>>();
        services.AddScoped<ICommandHandler<UpdateServiceCommand<TService>, ServiceResponse?>, UpdateServiceCommandHandler<TService>>();
        services.AddScoped<ICommandHandler<DeleteServiceCommand<TService>, bool>, DeleteServiceCommandHandler<TService>>();
        return services;
    }

    private static IServiceCollection AddFieldUpdateCrud<TFlight>(this IServiceCollection services)
        where TFlight : Aero.Flights.Domain.Ops.Flight
    {
        services.AddScoped<ICommandHandler<UpdateFlightStatusCommand<TFlight>, bool>, UpdateFlightStatusCommandHandler<TFlight>>();
        services.AddScoped<ICommandHandler<UpdateFlightTerminalCommand<TFlight>, bool>, UpdateFlightTerminalCommandHandler<TFlight>>();
        services.AddScoped<ICommandHandler<UpdateFlightRegistrationCommand<TFlight>, bool>, UpdateFlightRegistrationCommandHandler<TFlight>>();
        return services;
    }
}

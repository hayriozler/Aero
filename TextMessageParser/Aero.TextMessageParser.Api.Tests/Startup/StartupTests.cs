using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aero.TextMessageParser.Api.Tests.Startup;

public class StartupTests
{
    [Fact(DisplayName = "Bug 1: the real Program.cs host registers the element map and the BsmParser handler")]
    [Trait("Bug", "1")]
    public void Program_host_registers_element_map_and_message_handlers()
    {
        const string Why = "Originally Program.cs never called InstallElements/InstallMessages, so the element map and BsmParser could not be resolved. The test boots the real Program through WebApplicationFactory (processing hosted service removed and DB swapped to InMemory so only registrations are examined) and resolves both. It is green once Program.cs registers them (observed green on the current source).";
        using var factory = new ApiFactory(removeProcessingService: true);
        var elementMap = factory.Services.GetService<IReadOnlyDictionary<string, Element>>();
        var bsm = factory.Services.GetService<BsmParser>();
        Check.True(elementMap is not null, "IReadOnlyDictionary<string, Element> is not registered. " + Why);
        Check.True(bsm is not null, "BsmParser is not registered. " + Why);
    }

    [Fact(DisplayName = "Bug 1: MessageProcessingService can be constructed from the real host container and every message type has a keyed IMessageProcessor")]
    [Trait("Bug", "1")]
    public void Processing_service_dependencies_resolve()
    {
        const string Why = "The hosted service needs LoggerService and IServiceScopeFactory from the container, and ProcessMessageAsync looks up IMessageProcessor by key (BSM, BSMCHG, BSMDEL, BUM, BCMFOM, BCMFCM, BCMDBM).";
        using var factory = new ApiFactory(removeProcessingService: true);
        var service = ActivatorUtilities.CreateInstance<MessageProcessingService>(factory.Services);
        Check.True(service is not null, Why);
        using var scope = factory.Services.CreateScope();
        foreach (var key in new[] { "BSM", "BSMCHG", "BSMDEL", "BUM", "BCMFOM", "BCMFCM", "BCMDBM" })
            Check.True(scope.ServiceProvider.GetKeyedService<IMessageProcessor>(key) is not null, $"No IMessageProcessor registered for key {key}. {Why}");
    }

    [Fact(DisplayName = "Bug 1: the host starts with MessageProcessingService enabled", Skip = "Starting the hosted service runs ExecuteUpdateAsync and a PostgreSQL-only raw SQL claim query, neither of which InMemory supports, so the background service faults and stops the host in a test. Needs PostgreSQL. Reproduction: new ApiFactory(removeProcessingService: false).CreateClient() against a real database and assert no exception.")]
    [Trait("Bug", "1")]
    public void Host_starts_with_processing_service()
    {
        using var factory = new ApiFactory(removeProcessingService: false);
        using var client = factory.CreateClient();
    }

    [Fact(DisplayName = "Bug 15 (additional finding): the production TextMessageParserDbContext model can be built")]
    [Trait("Bug", "15")]
    public void Production_model_builds()
    {
        const string Why = "DataStoreContext declares DbSets for User, Role, Permission, etc. but applies configurations only from GetType().Assembly (the Api assembly), so those shared entities have no key configuration and model creation throws 'The entity type Permission requires a primary key'. Every first DbContext use (endpoint save, processing service) fails with any provider. Other tests in this project use a TestParserDbContext that ignores the foreign entities, otherwise nothing could run.";
        using var db = TestDbContextFactory.CreateProductionContext();
        var ex = Record.Exception(() => db.Model);
        Check.True(ex is null, $"Model creation threw {ex?.GetType().Name}: {ex?.Message}. {Why}");
    }

    [Fact(DisplayName = "Baseline: InstallElements + InstallMessages are self-consistent and resolve BsmParser with the shared element map")]
    [Trait("Kind", "Baseline")]
    public void Install_extensions_resolve_bsm()
    {
        var services = new ServiceCollection();
        services.InstallElements().InstallMessages();
        using var provider = services.BuildServiceProvider();
        Check.True(provider.GetService<BsmParser>() is not null, "BsmParser must resolve once both Install* extensions have run.");
        Check.True(provider.GetService<IReadOnlyDictionary<string, Element>>()?.Count == 24, "The element map must contain the 24 element parsers.");
    }
}

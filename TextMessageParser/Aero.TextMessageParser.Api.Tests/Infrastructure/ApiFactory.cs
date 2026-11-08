using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal sealed class ApiFactory(bool removeProcessingService) : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TextMessageParserDbContext>();
            services.RemoveAll<DbContextOptions<TextMessageParserDbContext>>();
            foreach (var d in services.Where(d => d.ServiceType.IsGenericType && d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration") && d.ServiceType.GenericTypeArguments[0] == typeof(TextMessageParserDbContext)).ToList())
                services.Remove(d);
            services.AddSingleton(_ => new DbContextOptionsBuilder<TextMessageParserDbContext>().UseInMemoryDatabase(_dbName).Options);
            services.AddScoped<TextMessageParserDbContext, TestParserDbContext>();
            if (removeProcessingService)
            {
                foreach (var d in services.Where(d => d.ServiceType == typeof(IHostedService)
                             && d.ImplementationType == typeof(MessageProcessingService)).ToList())
                    services.Remove(d);
            }
        });
    }
}

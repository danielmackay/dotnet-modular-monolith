using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi;
using Xunit.Abstractions;

namespace Common.Tests.Common;

/// <summary>
/// Host builder (services, DI, and configuration) for integration tests
/// </summary>
public class WebApiTestFactory : WebApplicationFactory<IWebApiMarker>
{
    public DatabaseContainer Database { get; } = new();

    public ITestOutputHelper? Output { private get; set; }

    public Lazy<HttpClient> AnonymousClient => new(CreateClient());

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Redirect application logging to test output
        builder.ConfigureLogging(x =>
        {
            x.ClearProviders();
            x.AddFilter(level => level >= LogLevel.Information);
            x.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(Output!));
        });

        builder.UseSetting("ConnectionStrings:customers", Database.ConnectionString);
        builder.UseSetting("ConnectionStrings:warehouse", Database.ConnectionString);
        builder.UseSetting("ConnectionStrings:catalog", Database.ConnectionString);
        builder.UseSetting("ConnectionStrings:orders", Database.ConnectionString);
    }
}
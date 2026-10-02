using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;

using Amiasea.Enterprise.Common;
using Amiasea.Enterprise.Common.Configuration;
using Amiasea.Enterprise.Core.Data.Entity;

namespace Amiasea.Enterprise.Test.Core.Api;

public sealed class TestHost : IAsyncDisposable
{
    private readonly WebApplication _application;

    private TestHost(
        WebApplication application)
    {
        _application = application;
    }

    public HttpClient CreateClient()
    {
        return _application.GetTestClient();
    }

    public IServiceScope CreateScope()
    {
        return _application.Services.CreateScope();
    }

    public static async Task<TestHost> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        // Plugin discovery is a Host concern.
        var pluginProvider = new TestCapabilityPluginProvider();

        builder.Services.AddSingleton<
            IEnterpriseCapabilityPluginProvider>(
                pluginProvider);

        var pluginAssemblies =
            await pluginProvider.GetPluginAssembliesAsync(
                cancellationToken);

        // Core API infrastructure is composed by the Host while the
        // service collection is still mutable.
        builder.Services.AddSingleton<
            IValidateOptions<IdentityOptions>,
            IdentityOptionsValidator>();

        builder.Services
            .AddOptions<IdentityOptions>()
            .BindConfiguration(IdentityOptions.SectionName)
            .ValidateOnStart();

        builder.Services.AddSingleton<IdentityCredentials>();
        builder.Services.AddSingleton<SqlAuthentication>();
        builder.Services.AddSingleton<SqlConnectionInterceptor>();

        builder.Host.UseWolverine(options =>
        {
            options.ApplicationAssembly =
                typeof(
                    Amiasea.Enterprise.Core.Api.Features.Capabilities.Endpoints.Endpoints)
                    .Assembly;

            foreach (var pluginAssembly in pluginAssemblies)
            {
                options.Discovery
                    .IncludeAssembly(pluginAssembly);
            }

            options.Discovery.IncludeHandlerModules = true;

            options.Services.AddSingleton(connection);

            options.Services.AddDbContextWithWolverineIntegration<
                EnterpriseDbContext>(
                db => db.UseSqlite(connection));
        });

        builder.Services.AddWolverineHttp();

        var application = builder.Build();

        await using (var scope =
            application.Services.CreateAsyncScope())
        {
            var db =
                scope.ServiceProvider
                    .GetRequiredService<EnterpriseDbContext>();

            await db.Database.EnsureCreatedAsync(
                cancellationToken);
        }

        application.MapWolverineEndpoints();

        await application.StartAsync(cancellationToken);

        return new TestHost(application);
    }

    public async ValueTask DisposeAsync()
    {
        await _application.StopAsync();
        await _application.DisposeAsync();
    }
}
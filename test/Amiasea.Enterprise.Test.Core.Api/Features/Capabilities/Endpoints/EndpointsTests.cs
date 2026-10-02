using System.Net;
using Amiasea.Enterprise.Core.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace Amiasea.Enterprise.Test.Core.Api;

[Trait("TestType", "Endpoint")]
public sealed class EndpointsTests
    : IClassFixture<EndpointsFixture>
{
    private readonly EndpointsFixture _fixture;
    private readonly HttpClient _client;

    public EndpointsTests(EndpointsFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Host!.CreateClient();
    }

    [Fact]
    public async Task Get_returns_capabilities()
    {
        using var scope = _fixture.Host!.CreateScope();

        var db =
            scope.ServiceProvider.GetRequiredService<EnterpriseDbContext>();

        var capability = new Amiasea.Enterprise.Core.Data.Entity.Capability
        {
            Name = "Test",
            Version = "1"
        };

        db.Capabilities.Add(capability);

        await db.SaveChangesAsync(
            TestContext.Current.CancellationToken);

        var response = await _client.GetAsync(
            "/capabilities",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var capabilities =
            await response.Content.ReadFromJsonAsync<
                IReadOnlyList<Amiasea.Enterprise.Core.Data.Entity.Capability>>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(capabilities);
        Assert.Single(capabilities);

        var result = capabilities[0];

        Assert.Equal(capability.ID, result.ID);
        Assert.Equal(capability.Name, result.Name);
        Assert.Equal(capability.Version, result.Version);
    }
}
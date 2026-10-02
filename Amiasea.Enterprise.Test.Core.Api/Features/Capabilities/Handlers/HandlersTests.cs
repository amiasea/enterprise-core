using Amiasea.Enterprise.Core.Data.Entity;
using Amiasea.Enterprise.Core.Api.Features.Capabilities.Handlers;
using Amiasea.Enterprise.Core.Api.Features.Capabilities.Commands;

namespace Amiasea.Enterprise.Test.Core.Api;

[Trait("TestType", "Handler")]
public sealed class HandlersTests
    : IClassFixture<HandlersFixture>
{
    private readonly EnterpriseDbContext _db;

    public HandlersTests(HandlersFixture fixture)
    {
        _db = fixture.Db;
    }

    [Fact]
    public async Task ConsumeAsync_returns_capabilities()
    {
        var capability = new Amiasea.Enterprise.Core.Data.Entity.Capability
        {
          Name = "Test",
          Version = "1"
        };

        _db.Capabilities.Add(capability);

        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Handlers.ConsumeAsync(
            new GetCapabilitiesCommand(),
            _db,
            TestContext.Current.CancellationToken);

        var capabilities = result.ToList();

        Assert.Single(capabilities);

        var resultCap = capabilities[0];

        Assert.Equal(capability.ID, resultCap.ID);
        Assert.Equal(capability.Name, resultCap.Name);
        Assert.Equal(capability.Version, resultCap.Version);
    }
}
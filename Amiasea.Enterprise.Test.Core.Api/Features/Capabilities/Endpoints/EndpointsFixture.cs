namespace Amiasea.Enterprise.Test.Core.Api;

public sealed class EndpointsFixture : IAsyncLifetime
{
    public TestHost? Host { get; private set; }

    public async ValueTask InitializeAsync()
    {
        Host = await TestHost.CreateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Host is not null)
        {
            await Host.DisposeAsync();
        }
    }
}
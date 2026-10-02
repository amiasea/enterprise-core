using System.Reflection;
using Amiasea.Enterprise.Common;
using Amiasea.Enterprise.Test.Capability.Api;

namespace Amiasea.Enterprise.Test.Core.Api;

public sealed class TestCapabilityPluginProvider
    : IEnterpriseCapabilityPluginProvider
{
    public Task<Assembly[]> GetPluginAssembliesAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            new[]
            {
                typeof(EnterpriseCapabilityExtension).Assembly
            });
    }
}
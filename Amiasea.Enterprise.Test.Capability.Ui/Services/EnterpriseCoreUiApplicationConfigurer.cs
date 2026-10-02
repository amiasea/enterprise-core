using Microsoft.AspNetCore.Builder;

using Amiasea.Enterprise.Common;

namespace Amiasea.Enterprise.Test.Capability.Ui;

public sealed class EnterpriseCoreUiApplicationConfigurer
    : IEnterpriseCapabilityApplicationConfigurer
{
    public ValueTask ConfigureAsync(WebApplication application, CancellationToken cancellationToken = default)
    {
        application
            .MapRazorComponents<EnterpriseCapability>()
            .AddInteractiveServerRenderMode();

        return ValueTask.CompletedTask;
    }
}
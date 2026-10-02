using Microsoft.Extensions.DependencyInjection;
using Wolverine;

using Amiasea.Enterprise.Common;

namespace Amiasea.Enterprise.Test.Capability.Ui;

public sealed class EnterpriseTestCapabilityUiExtension : IAsyncWolverineExtension
{
    public ValueTask Configure(WolverineOptions options)
    {
        options.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        options.Services.AddSingleton<
            IEnterpriseCapabilityApplicationConfigurer,
            EnterpriseCoreUiApplicationConfigurer>();

        return ValueTask.CompletedTask;
    }
}
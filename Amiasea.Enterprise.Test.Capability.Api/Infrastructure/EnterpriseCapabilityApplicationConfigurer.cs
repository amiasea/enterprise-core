using Amiasea.Enterprise.Common;
using Amiasea.Enterprise.Test.Capability.Ui;
using Microsoft.AspNetCore.Builder;

namespace Amiasea.Enterprise.Test.Capability.Api;

public sealed class EnterpriseCapabilityApplicationConfigurer
: IEnterpriseCapabilityApplicationConfigurer
{
  public ValueTask ConfigureAsync(
  WebApplication application,
  CancellationToken cancellationToken = default)
  {
    application.MapRazorComponents<EnterpriseCapability>();
    
    return ValueTask.CompletedTask;
  }
}

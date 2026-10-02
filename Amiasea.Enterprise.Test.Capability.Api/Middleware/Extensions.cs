using Microsoft.Extensions.DependencyInjection;
using Wolverine;

using Amiasea.Enterprise.Common;

namespace Amiasea.Enterprise.Test.Capability.Api;

public sealed class EnterpriseTestCapabilityApiExtension : IAsyncWolverineExtension
{
  public async ValueTask Configure(WolverineOptions options)
  {
  }
}
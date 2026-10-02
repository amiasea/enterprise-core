namespace Amiasea.Enterprise.Core.Api.Features.Capabilities.Endpoints;

using Amiasea.Enterprise.Core.Api.Features.Capabilities.Commands;
using Amiasea.Enterprise.Core.Data.Entity;
using Wolverine;
using Wolverine.Http;

public static class Endpoints
{
    [WolverineGet("/capabilities")]
    public static Task<IReadOnlyList<Capability>> Get(
        GetCapabilitiesCommand command,
        IMessageBus bus)
    {
        return bus.InvokeAsync<IReadOnlyList<Capability>>(command);
    }
}
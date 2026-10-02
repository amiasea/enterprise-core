namespace Amiasea.Enterprise.Core.Api.Features.Capabilities.Handlers;

using Amiasea.Enterprise.Core.Data.Entity;
using Amiasea.Enterprise.Core.Api.Features.Capabilities.Commands;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

[WolverineHandler]
public static class Handlers
{
    public static async Task<IReadOnlyList<Capability>> ConsumeAsync(
        GetCapabilitiesCommand command,
        EnterpriseDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.Capabilities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
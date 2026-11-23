using AqLife.Application.Abstractions.Persistence.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.DevelopmentPlan.Handler;

public sealed class ArchiveDevelopmentPlanCommandHandler(IPlanPersistence persistence) : IRequestHandler<ArchiveDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(ArchiveDevelopmentPlanCommand q, CancellationToken ct)
    {
        var p = await persistence.DevelopmentPlans.FirstAsync(x => x.UID == q.UID, ct);
        p.Archive();
        return p.UID;
    }
}

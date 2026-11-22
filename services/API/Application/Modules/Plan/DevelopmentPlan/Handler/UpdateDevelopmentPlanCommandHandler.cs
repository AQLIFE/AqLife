using AqLife.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Plan.DevelopmentPlan;
namespace Plan.DevelopmentPlan.Handler;

public sealed class UpdateDevelopmentPlanCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(UpdateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var p = await db.DevelopmentPlans.FirstAsync(x => x.UID == q.UID, ct);
        p.Update(q.Name, q.Description);
        return p.UID;
    }
}

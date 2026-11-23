using AqLife.Application.Abstractions.Persistence.Modules;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.DevelopmentPlan.Handler;

public sealed class UpdateDevelopmentPlanCommandHandler(IPlanPersistence persistence) : IRequestHandler<UpdateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(UpdateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var p = await persistence.DevelopmentPlans.FirstAsync(x => x.UID == q.UID, ct);
        p.Update(q.Name, q.Description);
        return p.UID;
    }
}

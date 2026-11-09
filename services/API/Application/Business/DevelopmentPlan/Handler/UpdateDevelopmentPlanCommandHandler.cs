using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Handler;

public sealed class UpdateDevelopmentPlanCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(UpdateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var p = await db.DevelopmentPlans.FirstAsync(x => x.UID == q.UID, ct);
        p.Update(q.Name, q.Description);
        return p.UID;
    }
}

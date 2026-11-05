using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Handler;
public sealed class CreateDevelopmentPlanCommandHandler(IApplicationDbContext db):IRequestHandler<CreateDevelopmentPlanCommand,Guid>{
    public async Task<Guid> Handle(CreateDevelopmentPlanCommand q,CancellationToken ct){
        var p=new DevelopmentPlanEntity(q.Name,q.Description);
        await db.DevelopmentPlans.AddAsync(p,ct);
        return p.UID;
    }
}

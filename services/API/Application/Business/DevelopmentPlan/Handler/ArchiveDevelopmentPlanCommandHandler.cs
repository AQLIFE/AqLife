using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Handler;
public sealed class ArchiveDevelopmentPlanCommandHandler(IApplicationDbContext db):IRequestHandler<ArchiveDevelopmentPlanCommand,Guid>{
    public async Task<Guid> Handle(ArchiveDevelopmentPlanCommand q,CancellationToken ct){
        var p=await db.DevelopmentPlans.FirstAsync(x=>x.UID==q.UID,ct);
        p.Archive();
        return p.UID;
    }
}

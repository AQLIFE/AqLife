using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Validator;
public sealed class ArchiveDevelopmentPlanValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):DevelopmentPlanValidatorBase<ArchiveDevelopmentPlanCommand>(db,"开发计划仍包含未归档任务，不能归档"){
    private protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand q,CancellationToken ct){
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        if(p.UID==DevelopmentPlanEntity.DefaultPlanId)return Reject(new DomainLegalityException("默认开发计划不能归档"));
        if(await Db.Todo.AnyAsync(x=>x.PlanId==p.UID&&!x.IsArchived,ct))return Reject(new DomainLegalityException("开发计划仍包含未归档任务，不能归档"));
        return true;
    }
}

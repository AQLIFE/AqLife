using AqLife.Domain.Command;
using AqLife.Domain.Services;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class SetTodoParentValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<SetTodoParentCommand>(db,"任务层级调整不合法"){
    private protected override async Task<bool> IsValidAsync(SetTodoParentCommand q,CancellationToken ct){
        var t=await Db.Todo.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);
        if(t is null)return Reject(new ResourceNotFoundException("Todo 不存在"));
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==t.PlanId,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        if(p.IsArchived)return Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
        var tasks=await Db.Todo.Where(x=>x.PlanId==t.PlanId).ToListAsync(ct);
        try{
            TodoHierarchyValidator.EnsureCanMove(t,q.ParentId,tasks);
        }
        catch(DomainLegalityException ex){
            return Reject(ex);
        }
        return true;
    }
}

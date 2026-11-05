using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Services;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class CreateTodoValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<CreateTodoCommand>(db,"开发计划或父任务不存在，或父任务不属于该计划"){
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand q,CancellationToken ct){
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==q.PlanId,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        if(p.IsArchived)return Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
        if(string.IsNullOrWhiteSpace(q.EffectiveTitle)||q.EffectiveTitle.Trim().Length>500)return Reject(new DomainLegalityException("任务标题不能为空且不能超过 500 个字符"));
        var tasks=await Db.Todo.Where(x=>x.PlanId==q.PlanId).ToListAsync(ct);
        var t=new TodoEntity(q.PlanId,q.EffectiveTitle,q.Description,q.Priority,q.EffectiveParentId,q.SortOrder);
        try{
            TodoHierarchyValidator.EnsureCanMove(t,q.EffectiveParentId,tasks.Append(t).ToArray());
        }
        catch(DomainLegalityException ex){
            return Reject(ex);
        }
        return true;
    }
}

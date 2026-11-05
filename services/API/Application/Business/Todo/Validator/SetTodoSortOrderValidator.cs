using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class SetTodoSortOrderValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<SetTodoSortOrderCommand>(db,"任务不存在或所属计划不可修改"){
    private protected override async Task<bool> IsValidAsync(SetTodoSortOrderCommand q,CancellationToken ct){
        var t=await Db.Todo.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);
        if(t is null)return Reject(new ResourceNotFoundException("Todo 不存在"));
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==t.PlanId,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        return !p.IsArchived||Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
    }
}

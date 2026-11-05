using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class RestoreTodoValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<RestoreTodoCommand>(db,"当前任务不能恢复"){
    private protected override async Task<bool> IsValidAsync(RestoreTodoCommand q,CancellationToken ct){
        var t=await Db.Todo.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);
        if(t is null)return Reject(new ResourceNotFoundException("Todo 不存在"));
        var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==t.PlanId,ct);
        if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));
        if(p.IsArchived)return Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
        if(t.FTID.HasValue&&await Db.Todo.AnyAsync(x=>x.UID==t.FTID.Value&&x.IsArchived,ct))return Reject(new DomainLegalityException("请先恢复父任务，再恢复当前任务"));
        return true;
    }
}

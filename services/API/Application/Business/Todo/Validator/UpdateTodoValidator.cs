using AqLife.Domain.Command;using AqLife.Domain.Entities;using AqLife.Shared.Exceptions;using AqLife.Shared.Options;using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class UpdateTodoValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<UpdateTodoCommand>(db,"Todo 更新条件不合法"){
private protected override async Task<bool> IsValidAsync(UpdateTodoCommand q,CancellationToken ct){
var t=await Db.Todo.FirstOrDefaultAsync(x=>x.UID==q.UID,ct);if(t is null)return Reject(new ResourceNotFoundException("Todo 不存在"));
if(string.IsNullOrWhiteSpace(q.EffectiveTitle)||q.EffectiveTitle.Trim().Length>500)return Reject(new DomainLegalityException("任务标题不能为空且不能超过 500 个字符"));
if(!Enum.IsDefined(q.Status))return Reject(new DomainLegalityException("无效的任务状态"));
var p=await Db.DevelopmentPlans.FirstOrDefaultAsync(x=>x.UID==t.PlanId,ct);if(p is null)return Reject(new ResourceNotFoundException("开发计划不存在"));if(p.IsArchived)return Reject(new DomainLegalityException("已归档的开发计划不允许修改"));
if(q.Status==TodoStatus.Completed&&t.Status!=TodoStatus.Completed){var incomplete=await Db.TodoDependencies.Where(x=>x.PlanId==t.PlanId&&x.SuccessorId==t.UID).Join(Db.Todo,d=>d.PredecessorId,x=>x.UID,(d,x)=>x).AnyAsync(x=>x.Status!=TodoStatus.Completed,ct);if(incomplete)return Reject(new DomainLegalityException("仍有未完成的前置任务，当前任务不能完成"));}
return true;
}}
using AqLife.Domain.Command;using AqLife.Domain.Entities;using AqLife.Shared.Exceptions;using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class TodoQueryValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<TodoQuery>(db,"开发计划不存在"){
private protected override async Task<bool> IsValidAsync(TodoQuery q,CancellationToken ct){var id=q.PlanId??DevelopmentPlanEntity.DefaultPlanId;return await Db.DevelopmentPlans.AnyAsync(x=>x.UID==id,ct)||Reject(new ResourceNotFoundException("开发计划不存在"));}}
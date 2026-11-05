using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;
public sealed class TodoRoadmapQueryValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db):TodoValidatorBase<TodoRoadmapQuery>(db,"开发计划不存在"){
    private protected override async Task<bool> IsValidAsync(TodoRoadmapQuery q,CancellationToken ct)=>await Db.DevelopmentPlans.AnyAsync(x=>x.UID==q.PlanId&&!x.IsArchived,ct)||Reject(new ResourceNotFoundException("开发计划不存在"));
}

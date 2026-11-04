using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Validator
{
    public sealed class CreateTodoValidator(IApplicationDbContext storage) : AbstractValidator<CreateTodoCommand>
    {
        private protected override string ErrorMessage { get; init; } = "开发计划或父任务不存在，或父任务不属于该计划";

        private protected override async Task<bool> IsValidAsync(CreateTodoCommand source, CancellationToken ct)
        {
            var planExists = await storage.DevelopmentPlans.AnyAsync(
                x => x.UID == source.PlanId && !x.IsArchived, ct);
            if (!planExists) return false;
            if (source.ParentId is null) return true;
            return await storage.Todo.AnyAsync(
                x => x.UID == source.ParentId && x.PlanId == source.PlanId && !x.IsArchived, ct);
        }
    }
}
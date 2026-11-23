using AqLife.Application.Plan.Contracts;
using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.DevelopmentPlan.Validator
{
    public class DevPlanValidator<TRequest>(IPlanPersistence persistence) : AbstractValidator<TRequest>
       where TRequest : IRequireDevPlan, ICommand
    {
        protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许增加,更新.删除任何任务";
        protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
            => await persistence.DevelopmentPlans.AnyAsync(e => e.UID == command.PlanId && !e.IsArchived, ct);

        protected override Exception CreateException(string message)
        => new DomainLegalityException(message);
    }
}

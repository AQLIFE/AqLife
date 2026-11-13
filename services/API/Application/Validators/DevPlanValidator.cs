using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Validators
{
    public class DevPlanValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest>
       where TRequest : IRequireDevPlan,ICommand
    {
        private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许增加,更新.删除任何任务";
        private protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
            => await storage.DevelopmentPlans.AnyAsync(e => e.UID  == command.PlanId && !e.IsArchived, ct);

        private protected override Exception CreateException(string message)
        => new DomainLegalityException(message);
    }
}

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
        private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.DevelopmentPlans.AnyAsync(e => e.UID  == query.PlanId && !e.IsArchived, ct);

        private protected override Exception CreateException(string message)
        => new DomainLegalityException(message);
    }

    /// <summary>
    /// 解决 B10 问题,任何Command只要继承了 IRequireDevPlan , IRequireValidEntity<TodoEntity>
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="storage"></param>
    public class DevPlanUpdateValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest>
       where TRequest : IRequireDevPlan,IRequireValidEntity<TodoEntity>,ICommand
    {
        private protected override string ErrorMessage { init; get; } = "请求的任务不属于该计划或该任务已存档";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => query is RestoreTodoCommand || await storage.Todo.AnyAsync(x=>x.UID == query.UID && x.PlanId == query.PlanId && !x.IsArchived,ct);

        private protected override Exception CreateException(string message)
        => new DomainLegalityException(message);
    }
}

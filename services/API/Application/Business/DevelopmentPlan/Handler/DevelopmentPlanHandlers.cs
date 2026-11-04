using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.DevelopmentPlan.Handler
{
    public sealed class DevelopmentPlanQueryHandler(IApplicationDbContext db)
        : IRequestHandler<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>
    {
        public async Task<IReadOnlyList<DevelopmentPlanDto>> Handle(DevelopmentPlanQuery query, CancellationToken ct)
        {
            var plans = await db.DevelopmentPlans.AsNoTracking()
                .Where(x => query.IncludeArchived || !x.IsArchived)
                .OrderBy(x => x.CreatedAt).ToListAsync(ct);
            return plans.Select(ToDto).ToList();
        }

        internal static DevelopmentPlanDto ToDto(DevelopmentPlanEntity plan)
            => new(plan.UID, plan.Name, plan.Description, plan.IsArchived,
                plan.CreatedAt, plan.UpdatedAt, plan.DependencyVersion);
    }

    public sealed class CreateDevelopmentPlanCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateDevelopmentPlanCommand, Guid>
    {
        public async Task<Guid> Handle(CreateDevelopmentPlanCommand command, CancellationToken ct)
        {
            var plan = new DevelopmentPlanEntity(command.Name, command.Description);
            await db.DevelopmentPlans.AddAsync(plan, ct);
            return plan.UID;
        }
    }

    public sealed class UpdateDevelopmentPlanCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateDevelopmentPlanCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateDevelopmentPlanCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            plan.Rename(command.Name);
            plan.UpdateDescription(command.Description);
            return plan.UID;
        }
    }

    public sealed class ArchiveDevelopmentPlanCommandHandler(IApplicationDbContext db)
        : IRequestHandler<ArchiveDevelopmentPlanCommand, Guid>
    {
        public async Task<Guid> Handle(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            if (await db.Todo.AnyAsync(x => x.PlanId == plan.UID && !x.IsArchived, ct))
                throw new DomainLegalityException("开发计划仍包含未归档任务，不能归档");
            plan.Archive();
            return plan.UID;
        }
    }
}
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.DevPlan;
using MediatR;

namespace AqLife.Application.Modules.DevelopmentPlan.Handler;

public sealed class CreateDevelopmentPlanCommandHandler(
    IApplicationDbContext db,
    ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand> mapper) : IRequestHandler<CreateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(CreateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var plan = mapper.ToEntity(q);
        await db.DevelopmentPlans.AddAsync(plan, ct);
        return plan.UID;
    }
}
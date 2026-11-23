using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities.DevPlan;
using MediatR;

namespace AqLife.Application.Plan.DevelopmentPlan.Handler;

public sealed class CreateDevelopmentPlanCommandHandler(
    IPlanPersistence persistence,
    ICreateMapper<DevelopmentPlanEntity, CreateDevelopmentPlanCommand> mapper) : IRequestHandler<CreateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(CreateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var plan = mapper.ToEntity(q);
        await persistence.AddDevelopmentPlanAsync(plan, ct);
        return plan.UID;
    }
}
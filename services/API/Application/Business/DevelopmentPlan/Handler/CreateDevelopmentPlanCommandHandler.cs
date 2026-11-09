using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.DevelopmentPlan.Handler;

public sealed class CreateDevelopmentPlanCommandHandler(
    IApplicationDbContext db,
    DevelopmentPlanMapper mapper) : IRequestHandler<CreateDevelopmentPlanCommand, Guid>
{
    public async Task<Guid> Handle(CreateDevelopmentPlanCommand q, CancellationToken ct)
    {
        var plan = mapper.ToEntity(q);
        await db.DevelopmentPlans.AddAsync(plan, ct);
        return plan.UID;
    }
}
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.DevelopmentPlan.Handler;

public sealed class DevelopmentPlanQueryHandler(
    IApplicationDbContext db,
    DevelopmentPlanMapper mapper) : IRequestHandler<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>
{
    public async Task<IReadOnlyList<DevelopmentPlanDto>> Handle(DevelopmentPlanQuery q, CancellationToken ct)
    {
        var plans = await db.DevelopmentPlans.AsNoTracking()
            .Where(x => q.IncludeArchived || !x.IsArchived)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        return plans.Select(mapper.ToDto).ToList();
    }
}
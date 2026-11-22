using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Plan.DevelopmentPlan;
using Plan.Todo;

namespace Plan.DevelopmentPlan.Handler;

public sealed class DevelopmentPlanQueryHandler(ISearch<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>, DevelopmentPlanEntity> searcher,
    IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto> mapper) : IRequestHandler<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>
{
    public async Task<IReadOnlyList<DevelopmentPlanDto>> Handle(DevelopmentPlanQuery q, CancellationToken ct)
    {
        IQueryable<DevelopmentPlanEntity> plans = await searcher.SearchAsync(q, ct);
        var result = await plans.ToListAsync(ct);
        return [.. result.Select(mapper.ToDto)];
    }
}
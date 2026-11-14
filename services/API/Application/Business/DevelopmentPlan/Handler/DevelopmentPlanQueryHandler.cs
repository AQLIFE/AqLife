using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.DevelopmentPlan.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.DevelopmentPlan.Handler;

public sealed class DevelopmentPlanQueryHandler(ISearch<DevelopmentPlanQuery, DevelopmentPlanEntity> searcher,
    IViewMapper<DevelopmentPlanEntity, DevelopmentPlanDto> mapper) : IRequestHandler<DevelopmentPlanQuery, IReadOnlyList<DevelopmentPlanDto>>
{
    public async Task<IReadOnlyList<DevelopmentPlanDto>> Handle(DevelopmentPlanQuery q, CancellationToken ct)
    {
        IQueryable<DevelopmentPlanEntity> plans = await searcher.SearchAsync(q, ct);
        var result =  await plans.ToListAsync(ct);
        return [..result.Select(mapper.ToDto)];
    }
}
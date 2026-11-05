using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Entities; using AqLife.Shared.IView; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Handler;
public sealed class DevelopmentPlanQueryHandler(IApplicationDbContext db):IRequestHandler<DevelopmentPlanQuery,IReadOnlyList<DevelopmentPlanDto>>{
public async Task<IReadOnlyList<DevelopmentPlanDto>> Handle(DevelopmentPlanQuery q,CancellationToken ct){var rows=await db.DevelopmentPlans.AsNoTracking().Where(x=>q.IncludeArchived||!x.IsArchived).OrderBy(x=>x.CreatedAt).ToListAsync(ct);return rows.Select(x=>new DevelopmentPlanDto(x.UID,x.Name,x.Description,x.IsArchived,x.CreatedAt,x.UpdatedAt,x.DependencyVersion)).ToList();}
}
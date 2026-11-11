using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public sealed class DevelopmentPlanSearch<TDevPlanQuery>(IApplicationDbContext storage, IHttpContextAccessor httpContext, QueryMapper queryMapper,
            IEnumerable<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<TDevPlanQuery, DevelopmentPlanEntity, EntitySearchCriteria>(storage, httpContext, searchStrategies)
        where TDevPlanQuery : class, IReadDevPlan
    {
        protected private override EntitySearchCriteria MapToCriteria(TDevPlanQuery query)
        => query switch
        {
            DevelopmentPlanQuery planQuery => queryMapper.ToCriteria(planQuery),
            TodoRoadmapQuery roadmapQuery => queryMapper.ToCriteria(roadmapQuery),
            _ => throw new RequestFailException("Invalid query type")
        };

        protected private override IQueryable<DevelopmentPlanEntity> BuildBaseQuery(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        {
            queryable = IsValid ? queryable : queryable.Where(e => !e.IsArchived);
            return query switch
            {
                DevelopmentPlanQuery planQuery => queryable,
                TodoRoadmapQuery roadmapQuery => queryable.Include(e => IsValid ? e.Todos : e.Todos.Where(to => !to.IsArchived)).ThenInclude(todo => todo.LayoutEntity)
                    .Include(e => e.Todos).ThenInclude(todo => todo.IncomingDependencies)
                    .Include(e => e.Todos).ThenInclude(todo => todo.OutgoingDependencies).AsSplitQuery(),
                _ => throw new RequestFailException("Invalid query type")
            };
        }
        protected private override IQueryable<DevelopmentPlanEntity> ApplyDefaultOrder(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        => queryable.OrderBy(e => e.Name);// 对于继承了IReadDevPlan的任何 Query 这点都不重要

        //private protected override IQueryable<DevelopmentPlanEntity> ApplyResultPolicy(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        //=> IsValid ? queryable : queryable.Where(e => !e.IsArchived);
    }
}

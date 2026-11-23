using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Plan.Contracts;
using Core.Search;
using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.DevelopmentPlan.Search
{
    public sealed class DevelopmentPlanSearch<TDevPlanQuery, TResource>(IApplicationDbContext dbContext, IHttpContextAccessor httpContext,
        ISearchMapper<TDevPlanQuery, TResource, EntitySearchCriteria> queryMapper,
        IEnumerable<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<TDevPlanQuery, TResource, DevelopmentPlanEntity, EntitySearchCriteria>(dbContext, httpContext, searchStrategies)
        where TDevPlanQuery : class, IReadDevPlan, IQuery<TResource>
        where TResource : class
    {
        protected override EntitySearchCriteria MapToCriteria(TDevPlanQuery query)
        => queryMapper.ToCriteria(query);

        IQueryable<DevelopmentPlanEntity> BuildBaseQueryInternal(IQueryable<DevelopmentPlanEntity> queryable)
        {
            queryable = queryable
               .Include(e => e.Todos.Where(todo => IsValid || !todo.IsArchived))
                   .ThenInclude(todo => todo.LayoutEntity)
               .Include(e => e.Todos.Where(todo => IsValid || !todo.IsArchived))
                   .ThenInclude(todo => todo.IncomingDependencies)
                        .ThenInclude(dependency => dependency.PredecessorTodo)
               .Include(e => e.Todos.Where(todo => IsValid || !todo.IsArchived))
                   .ThenInclude(todo => todo.OutgoingDependencies)
                         .ThenInclude(dependency => dependency.SuccessorTodo);

            return queryable.AsSplitQuery();
        }

        protected override IQueryable<DevelopmentPlanEntity> BuildBaseQuery(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        {
            queryable = IsValid ? queryable : queryable.Where(e => !e.IsArchived);
            return query switch
            {
                DevelopmentPlanQuery planQuery => queryable,
                TodoRoadmapQuery roadmapQuery => BuildBaseQueryInternal(queryable),
                _ => throw new RequestFailException("Invalid query type")
            };
        }
        protected override IQueryable<DevelopmentPlanEntity> ApplyDefaultOrder(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        => queryable.OrderBy(e => e.CreatedAt);// 按创建时间查找,这样适应人类操作直觉

        //protected override IQueryable<DevelopmentPlanEntity> ApplyResultPolicy(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        //=> IsValid ? queryable : queryable.Where(e => !e.IsArchived);
    }
}

using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public sealed class DevelopmentPlanSearch<TDevPlanQuery, TResource>(IApplicationDbContext storage, IHttpContextAccessor httpContext,
        ISearchMapper<TDevPlanQuery, TResource, EntitySearchCriteria> queryMapper,
        IEnumerable<ISearchStrategy<DevelopmentPlanEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<TDevPlanQuery, DevelopmentPlanEntity, EntitySearchCriteria>(storage, httpContext, searchStrategies)
        where TDevPlanQuery : class, IReadDevPlan, IQuery<TResource>
        where TResource : class
    {
        protected private override EntitySearchCriteria MapToCriteria(TDevPlanQuery query)
        => queryMapper.ToCriteria(query);

        private IQueryable<DevelopmentPlanEntity> BuildBaseQueryInternal(IQueryable<DevelopmentPlanEntity> queryable)
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

        protected private override IQueryable<DevelopmentPlanEntity> BuildBaseQuery(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        {
            queryable = IsValid ? queryable : queryable.Where(e => !e.IsArchived);
            return query switch
            {
                DevelopmentPlanQuery planQuery => queryable,
                TodoRoadmapQuery roadmapQuery => BuildBaseQueryInternal(queryable),
                _ => throw new RequestFailException("Invalid query type")
            };
        }
        protected private override IQueryable<DevelopmentPlanEntity> ApplyDefaultOrder(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        => queryable.OrderBy(e => e.CreatedAt);// 按创建时间查找,这样适应人类操作直觉

        //private protected override IQueryable<DevelopmentPlanEntity> ApplyResultPolicy(IQueryable<DevelopmentPlanEntity> queryable, TDevPlanQuery query)
        //=> IsValid ? queryable : queryable.Where(e => !e.IsArchived);
    }
}

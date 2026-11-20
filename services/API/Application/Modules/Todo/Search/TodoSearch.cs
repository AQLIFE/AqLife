using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AqLife.Application.Modules.Todo.Search
{
    public sealed class TodoSearch(ISearchMapper<TodoQuery, PageResult<TodoDto>, TodoSearchCriteria> queryMapper, IApplicationDbContext storage, IHttpContextAccessor httpContext,
        IEnumerable<ISearchStrategy<TodoEntity, TodoSearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery, PageResult<TodoDto>, TodoEntity, TodoSearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected override TodoSearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<TodoEntity> ApplyDefaultOrder(IQueryable<TodoEntity> queryble, TodoQuery query)
        => queryble.OrderBy(e => e.SortOrder);

        protected override IQueryable<TodoEntity> BuildBaseQuery(IQueryable<TodoEntity> queryable, TodoQuery query)
        => queryable
            .Include(e => e.IncomingDependencies)
                .ThenInclude(dependency => dependency.PredecessorTodo)
            .Include(e => e.OutgoingDependencies)
                .ThenInclude(dependency => dependency.SuccessorTodo)
            .Include(e => e.LayoutEntity)
            .Include(e => e.Plan)
            .AsSplitQuery();

        protected override IQueryable<TodoEntity> ApplyResultPolicy(IQueryable<TodoEntity> queryable, TodoQuery query)
        => IsValid ? queryable : queryable.Where(e => !e.IsArchived);
    }
}

using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AqLife.Application.Business.Todo.Search
{
    public sealed class TodoSearch(ISearchMapper<TodoQuery, PageResult<TodoDto>, TodoSearchCriteria> queryMapper, IApplicationDbContext storage, IHttpContextAccessor httpContext,
        IEnumerable<ISearchStrategy<TodoEntity, TodoSearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery, TodoEntity, TodoSearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected private override TodoSearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<TodoEntity> ApplyDefaultOrder(IQueryable<TodoEntity> queryble, TodoQuery query)
        => queryble.OrderBy(e => e.SortOrder);

        protected private override IQueryable<TodoEntity> BuildBaseQuery(IQueryable<TodoEntity> queryable, TodoQuery query)
        => queryable.Include(e =>e.IncomingDependencies).Include(e => e.OutgoingDependencies).Include(e=>e.LayoutEntity).Include(e=>e.Plan).AsSplitQuery();

        private protected override IQueryable<TodoEntity> ApplyResultPolicy(IQueryable<TodoEntity> queryable, TodoQuery query)
        => IsValid ? queryable : queryable.Where(e => !e.IsArchived);
    }
}

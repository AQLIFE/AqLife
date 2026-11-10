using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AqLife.Application.Business.Todo.Search
{
    public class TodoSearch(QueryMapper queryMapper, IApplicationDbContext storage,IHttpContextAccessor httpContext,
        IEnumerable<ISearchStrategy<TodoEntity, EntitySearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery, TodoEntity, TodoDto, EntitySearchCriteria>(storage, httpContext,searchStrategies)
    {
        protected private override EntitySearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<TodoEntity> ApplyDefaultOrder(IQueryable<TodoEntity> queryble, TodoQuery query)
        => queryble.OrderBy(e => e.CreatedAt);

        protected private override IQueryable<TodoEntity> BuildBaseQuery(IQueryable<TodoEntity> queryable, TodoQuery query)
       => query.IsTree ? queryable.Where(t => t.FTID == null).Include(e => e.Children) : queryable;
    }
}

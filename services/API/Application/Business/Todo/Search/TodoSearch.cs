using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Mappers;


namespace AqLife.Application.Business.Todo.Search
{
    public class TodoSearch(QueryMapper queryMapper, IApplicationDbContext storage
        , IEnumerable<ISearchStrategy<TodoEntity, EntitySearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery, TodoEntity, TodoDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TodoEntity>> BuildBaseQueryAsync(IQueryable<TodoEntity> queryable, TodoQuery query)
       => query.IsTree ? queryable.Where(t => t.FTID == null).Include(e => e.Children) : queryable;
    }
}

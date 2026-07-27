using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mappers;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using MyLife.Application.Abstractions.Persistence;


namespace MyLife.Application.Business.Todo.Search
{
    public class TodoSearch(QueryMapper queryMapper, IApplicationDbContext storage
        , IEnumerable<ISearchStrategy<TodoEntity, EntitySearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery, TodoEntity, TodoDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TodoEntity>> BuildBaseQueryAsync(IQueryable<TodoEntity> queryable, TodoQuery query)
       => queryable.Include(e => e.TodoList);
    }
}

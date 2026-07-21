using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
using MyLife.Service.Mapper;
using MyLife.Service.Search.Base;
using MyLife.Shared.IView;


namespace MyLife.Service.Search.Todo
{
    public class TodoSearch(QueryMapper queryMapper, AppStorage storage
        , IEnumerable<ISearchStrategy<TodoEntity,EntitySearchCriteria>> searchStrategies
        ) : BaseSearch<TodoQuery,TodoEntity,TodoDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TodoQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TodoEntity>> BuildBaseQueryAsync(IQueryable<TodoEntity> queryable,TodoQuery query)
       => queryable.Include(e => e.TodoList);
    }
}

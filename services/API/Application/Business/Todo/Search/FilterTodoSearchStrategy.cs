using MyLife.Application.Search;
using MyLife.Domain.Entities;

namespace MyLife.Application.Business.Todo.Search
{

    public class FilterTodoSearchStrategy : FilteredSearchStrategyBase<TodoEntity, EntitySearchCriteria>
    {
        protected override IQueryable<TodoEntity> ApplyKeywordFilter(
        IQueryable<TodoEntity> q, string keyword)
        => q.Where(e => e.Desc.Contains(keyword));
    }
}

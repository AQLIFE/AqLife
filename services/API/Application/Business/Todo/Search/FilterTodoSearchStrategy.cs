using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class FilterTodoSearchStrategy : FilteredSearchStrategyBase<TodoEntity, EntitySearchCriteria>
    {
        protected private override IQueryable<TodoEntity> ApplyKeywordFilter(IQueryable<TodoEntity> q, string keyword)
            => q.Where(e => e.Title.Contains(keyword));
    }
}
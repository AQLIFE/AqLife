using AqLife.Application.Search;
using AqLife.Domain.Entities;
using AqLife.Shared.Options;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class AllTodoSearchStrategy : AllSearchStrategyBase<TodoEntity, EntitySearchCriteria>
    {
        public override IQueryable<TodoEntity> Execute(
            IQueryable<TodoEntity> queryable, EntitySearchCriteria criteria, CancellationToken ct = default)
            => queryable.Where(e => e.Status != TodoStatus.Completed && !e.IsArchived);
    }
}
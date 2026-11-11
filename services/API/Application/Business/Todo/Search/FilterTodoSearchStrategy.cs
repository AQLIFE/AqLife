using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class FilterTodoSearchStrategy : FilteredSearchStrategyBase<TodoEntity, TodoSearchCriteria>
    {
        public override bool IsMatch(TodoSearchCriteria c)
            => base.IsMatch(c) || c.PlanId is not null;
        protected private override IQueryable<TodoEntity> ApplyKeywordFilter(IQueryable<TodoEntity> q, string keyword)
            => q.Where(e => e.Title.Contains(keyword));
        public override IQueryable<TodoEntity> Execute(IQueryable<TodoEntity> queryable, TodoSearchCriteria c, CancellationToken ct = default)
        => c switch
        {
            { UID: { } uid } => queryable.Where(e => e.UID == uid),
            { PlanId: { } planId } => queryable.Where(e => e.PlanId == planId),
            { Keyword: { } keyword } => ApplyKeywordFilter(queryable, keyword),
            _=>throw new RequestFailException("不合规的操作，该请求不应被处理")
        };
    }
}
using AqLife.Application.Search;
using AqLife.Domain.Entities;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class AllTodoSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<TodoEntity, EntitySearchCriteria>(httpContext)
    {
        public override IQueryable<TodoEntity> Execute(
            IQueryable<TodoEntity> queryable, EntitySearchCriteria criteria, CancellationToken ct = default)
            => IsValid ? queryable : queryable.Where(e => e.Status != TodoStatus.Completed && !e.IsArchived);
    }
}
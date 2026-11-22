using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;

namespace Plan.Todo.Search
{
    public sealed class AllTodoSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<TodoEntity, TodoSearchCriteria>(httpContext)
    {
        public override bool IsMatch(TodoSearchCriteria c)
            => base.IsMatch(c) && c.PlanId is null;
    }
}
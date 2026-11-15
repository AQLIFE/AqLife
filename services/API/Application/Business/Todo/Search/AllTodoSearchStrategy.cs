using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class AllTodoSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<TodoEntity, TodoSearchCriteria>(httpContext)
    {
        public override bool IsMatch(TodoSearchCriteria c)
            => base.IsMatch(c) && c.PlanId is null;
    }
}
using AqLife.Application.Search;
using AqLife.Domain.Entities;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Todo.Search
{
    public sealed class AllTodoSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<TodoEntity, EntitySearchCriteria>(httpContext)
    {
    }
}
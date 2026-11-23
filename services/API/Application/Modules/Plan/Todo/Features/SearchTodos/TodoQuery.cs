using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;
namespace AqLife.Application.Plan.Todo;
/// <summary>任务查询</summary>
public record TodoQuery(Guid? UID, string? Title, Guid? PlanId, int Page = 1, int PageSize = 50) : IPageQuery, IQuery<PageResult<TodoDto>>;

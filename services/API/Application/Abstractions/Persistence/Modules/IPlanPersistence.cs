using AqLife.Domain.Entities.DevPlan;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Plan 模块专属持久化入口，覆盖 DevelopmentPlan、Todo、依赖和路线布局。
/// </summary>
public interface IPlanPersistence
{
    IQueryable<DevelopmentPlanEntity> DevelopmentPlans { get; }
    IQueryable<TodoEntity> Todos { get; }
    IQueryable<TodoDependencyEntity> TodoDependencies { get; }
    IQueryable<RoadmapNodeLayoutEntity> RoadmapNodeLayouts { get; }

    Task AddDevelopmentPlanAsync(DevelopmentPlanEntity plan, CancellationToken cancellationToken = default);
    Task AddTodoAsync(TodoEntity todo, CancellationToken cancellationToken = default);
    Task AddTodoDependencyAsync(TodoDependencyEntity dependency, CancellationToken cancellationToken = default);
    Task AddRoadmapNodeLayoutAsync(RoadmapNodeLayoutEntity layout, CancellationToken cancellationToken = default);
    void RemoveTodo(TodoEntity todo);
    void RemoveTodoDependency(TodoDependencyEntity dependency);
}

using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AqLife.Domain.Command
{
    /// <summary>
    ///  任务 查询
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Title"></param>
    /// <param name="PlanId"></param>
    /// <param name="Page"></param>
    /// <param name="PageSize"></param>
    public record TodoQuery(
        Guid? UID,
        string? Title,
        Guid? PlanId,         // 每个 Todo 都必须归属与某个计划
        int Page = 1,
        int PageSize = 50
        ) : IPageQuery, IQuery<PageResult<TodoDto>>;
    /// <summary>
    /// 开发计划全量查询: 包含 任务 , 任务依赖 , 任务布局
    /// </summary>
    /// <param name="PlanId"></param>
    public record TodoRoadmapQuery(Guid PlanId) : IQuery<TodoRoadmapDto>;

    /// <summary>
    /// 添加 任务 : 自动添加 布局,设置对应计划组
    /// </summary>
    public record CreateTodoCommand : ICreateCommand, IRequireDevPlan
    {
        public Guid PlanId { get; init; } = DevelopmentPlanEntity.DefaultPlanId;
        [StringLength(500,ErrorMessage = "任务标题不能超过 500 个字符")] public string Title { get; init; } = string.Empty;
        [StringLength(10000,ErrorMessage = "任务描述长度不能超过 10000 个字符")] public string? Description { get; init; }
        [Range(0, int.MaxValue)] public int SortOrder { get; init; } = 0;
    }
    /// <summary>
    /// 更新 任务信息 : 标题  描述,任务状态
    /// </summary>
    public record UpdateTodoCommand : IRequireValidEntity<TodoEntity>,IRequireDevPlan ,IUpdateCommand
    {
        public Guid UID { get; init; }
        public Guid PlanId { get; init; }
        [StringLength(500)] public string Title { get; init; } = string.Empty;
        [StringLength(10000)] public string? Description { get; init; }
        [Range(0, int.MaxValue)] public int SortOrder { get; init; } = 0;
    }
    /// <summary>
    /// 更新 任务的优先级
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="SortOrder"></param>
    public record UpdateTodoStatusCommand(Guid UID,TodoStatus Status,Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand,IRequireDevPlan;
    /// <summary>
    /// 存档指定的任务
    /// </summary>
    /// <param name="UID"></param>
    public record ArchiveTodoCommand(Guid UID,Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand,IRequireDevPlan;
    /// <summary>
    /// 重启 特定的任务
    /// </summary>
    /// <param name="UID"></param>
    public record RestoreTodoCommand(Guid UID, Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand, IRequireDevPlan;
    /// <summary>
    /// 删除特定的任务
    /// </summary>
    /// <param name="UID"></param>
    public record DeleteTodoCommand(Guid UID,Guid PlanId) : IRequireValidEntity<TodoEntity>, IDeleteCommand,IRequireDevPlan;
    /// <summary>
    /// 添加 任务依赖
    /// </summary>
    /// <param name="PlanId"></param>
    /// <param name="PredecessorId"></param>
    /// <param name="SuccessorId"></param>
    public record AddTodoDependencyCommand(Guid PlanId, Guid PredecessorId, Guid SuccessorId) : ICreateCommand,IRequireDevPlan;
    /// <summary>
    /// 移除任务依赖
    /// </summary>
    /// <param name="PlanId"></param>
    /// <param name="PredecessorId"></param>
    /// <param name="SuccessorId"></param>
    public record RemoveTodoDependencyCommand(Guid UID,Guid PlanId) : IRequireValidEntity<TodoDependencyEntity>, IDeleteCommand,IRequireDevPlan;
    /// <summary>
    /// 更新 任务布局节点[x,y]
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="PlanId"></param>
    /// <param name="TodoId"></param>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    public record SaveRoadmapNodeLayoutCommand(Guid UID,Guid PlanId, Guid TodoId, double X, double Y) : IRequireValidEntity<RoadmapNodeLayoutEntity>, IUpdateCommand,IRequireDevPlan;

}
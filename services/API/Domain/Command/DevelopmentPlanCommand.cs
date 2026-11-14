using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace AqLife.Domain.Command
{
    /// <summary>
    /// 开发计划全量查询: 包含 任务 , 任务依赖 , 任务布局
    /// </summary>
    /// <param name="PlanId"></param>
    public record TodoRoadmapQuery(Guid PlanId) : IRequireValidEntity<DevelopmentPlanEntity>, IQuery<TodoRoadmapDto>,IReadDevPlan
    {
        /// <summary>
        /// 与 PlanId  具有相同含义,仅用于满足 IRequireValidEntity 依赖
        /// </summary>
        public Guid UID { get; init; } = PlanId;// 不需要手动填写
    }
    public record DevelopmentPlanQuery(Guid? UID=null,string? Title=null) : IQuery<IReadOnlyList<DevelopmentPlanDto>>,IReadDevPlan;
    public record CreateDevelopmentPlanCommand(
        [Required, StringLength(128, MinimumLength = 1)] string Name,
        [StringLength(2000)] string? Description = null) : ICreateCommand;
    public record UpdateDevelopmentPlanCommand(
        Guid UID,
        [Required, StringLength(128, MinimumLength = 1)] string Name,
        [StringLength(2000)] string? Description = null) : IRequireValidEntity<DevelopmentPlanEntity>, IRequireDevPlan,IUpdateCommand
    {
        /// <summary>
        /// 与 UID 具有相同含义,仅用于满足 IRequireDevPlan 依赖
        /// </summary>
        public Guid PlanId { get; init; } = UID; // 不需要手动填写
    }
    public record ArchiveDevelopmentPlanCommand(Guid UID) : IRequireValidEntity<DevelopmentPlanEntity>, IRequireDevPlan, IUpdateCommand
    {
        /// <summary>
        /// 与 UID 具有相同含义,仅用于满足 IRequireDevPlan 依赖
        /// </summary>
        public Guid PlanId { get; init; } = UID; // 不需要手动填写
    }
}
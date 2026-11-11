using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command
{
    public record DevelopmentPlanQuery(bool IncludeArchived = false) : IQuery<IReadOnlyList<DevelopmentPlanDto>>;
    public record CreateDevelopmentPlanCommand(
        [Required, StringLength(128, MinimumLength = 1)] string Name,
        [StringLength(2000)] string? Description = null) : ICreateCommand;
    public record UpdateDevelopmentPlanCommand(
        Guid UID,
        [Required, StringLength(128, MinimumLength = 1)] string Name,
        [StringLength(2000)] string? Description = null) : IRequireValidEntity<DevelopmentPlanEntity>, IUpdateCommand;
    public record ArchiveDevelopmentPlanCommand(Guid UID) : ICommand;
}
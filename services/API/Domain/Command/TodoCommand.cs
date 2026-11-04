using AqLife.Domain.CommandInterface;
using MediatR;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AqLife.Domain.Command
{
    public record TodoQuery(
        Guid? UID = null,
        string? Desc = null,
        bool IsTree = true,
        int Page = 1,
        int PageSize = 50,
        Guid? PlanId = null,
        bool IncludeCompleted = false,
        bool IncludeArchived = false,
        Guid? ParentId = null) : IPageQuery, IQuery<PageResult<TodoDto>>;

    // New clients use Title/ParentId/PlanId. Legacy Desc/FTID payloads continue
    // to target the stable default plan until the MGS client is upgraded.
    public record CreateTodoCommand : ICreateCommand
    {
        public Guid PlanId { get; init; } = DevelopmentPlanEntity.DefaultPlanId;
        [StringLength(256)] public string? Title { get; init; }
        [JsonPropertyName("Desc"), StringLength(256)] public string? LegacyDesc { get; init; }
        [StringLength(10000)] public string? Description { get; init; }
        [Range(1, 99)] public int Priority { get; init; } = 1;
        public Guid? ParentId { get; init; }
        [JsonPropertyName("FTID")] public Guid? LegacyParentId { get; init; }
        [Range(0, int.MaxValue)] public int SortOrder { get; init; }

        [JsonIgnore] public string EffectiveTitle => Title ?? LegacyDesc ?? string.Empty;
        [JsonIgnore] public Guid? EffectiveParentId => ParentId ?? LegacyParentId;
        [JsonIgnore] public string Desc => EffectiveTitle;
        [JsonIgnore] public Guid? FTID => EffectiveParentId;
    }

    public record UpdateTodoCommand : IRequireValidEntity<TodoEntity>, IUpdateCommand
    {
        public Guid UID { get; init; }
        [StringLength(256)] public string? Title { get; init; }
        [JsonPropertyName("Desc"), StringLength(256)] public string? LegacyDesc { get; init; }
        [StringLength(10000)] public string? Description { get; init; }
        [JsonConverter(typeof(JsonStringEnumConverter))] public TodoStatus Status { get; init; }
        [Range(1, 99)] public int Priority { get; init; } = 1;

        [JsonIgnore] public string EffectiveTitle => Title ?? LegacyDesc ?? string.Empty;
        [JsonIgnore] public string Desc => EffectiveTitle;
    }

    public record SetTodoParentCommand(Guid UID, Guid? ParentId) : ICommand;
    public record SetTodoSortOrderCommand(Guid UID, int SortOrder) : ICommand;
    public record ArchiveTodoCommand(Guid UID) : ICommand;
    public record RestoreTodoCommand(Guid UID) : ICommand;
    public record DeleteTodoCommand(Guid UID) : ICommand<Unit>;

    public record AddTodoDependencyCommand(Guid PlanId, Guid PredecessorId, Guid SuccessorId) : ICommand;
    public record RemoveTodoDependencyCommand(Guid PlanId, Guid PredecessorId, Guid SuccessorId) : ICommand;

    public record SaveRoadmapNodeLayoutCommand(Guid PlanId, Guid TodoId, double X, double Y) : ICommand;

    public record TodoRoadmapQuery(Guid PlanId) : IQuery<TodoRoadmapDto>;
}
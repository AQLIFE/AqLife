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

    public record CreateTodoCommand(
        Guid PlanId,
        [Required, StringLength(256, MinimumLength = 1)] string Title,
        string? Description = null,
        [Range(1, 99)] int Priority = 1,
        Guid? ParentId = null,
        [Range(0, int.MaxValue)] int SortOrder = 0) : ICreateCommand
    {
        [JsonIgnore] public string Desc => Title;
        [JsonIgnore] public Guid? FTID => ParentId;
    }

    public record UpdateTodoCommand(
        Guid UID,
        [Required, StringLength(256, MinimumLength = 1)] string Title,
        string? Description,
        [JsonConverter(typeof(JsonStringEnumConverter))] TodoStatus Status,
        [Range(1, 99)] int Priority) : IRequireValidEntity<TodoEntity>, IUpdateCommand
    {
        [JsonIgnore] public string Desc => Title;
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
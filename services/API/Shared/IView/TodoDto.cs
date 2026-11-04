using System.ComponentModel.DataAnnotations;

namespace AqLife.Shared.IView
{
    public record TodoDto(
        Guid UID,
        Guid? FTID,
        [Required] string Desc,
        [Required] string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset? CompletedAt,
        int Priority,
        ICollection<TodoDto> TodoList,
        bool HasChildren) : IEntityDto
    {
        public Guid PlanId { get; init; }
        public string Title { get; init; } = Desc;
        public string? Description { get; init; }
        public DateTimeOffset UpdatedAt { get; init; }
        public int SortOrder { get; init; }
        public bool IsArchived { get; init; }
        public bool IsBlocked { get; init; }
        public int IncompleteDependencyCount { get; init; }
        public IReadOnlyCollection<Guid> BlockingTodoIds { get; init; } = Array.Empty<Guid>();
    }

    public record TodoDependencyDto(Guid PredecessorId, Guid SuccessorId, DateTimeOffset CreatedAt);
    public record RoadmapNodeLayoutDto(Guid TodoId, double X, double Y, DateTimeOffset UpdatedAt);
    public record TodoRoadmapDto(
        Guid PlanId,
        string PlanName,
        long DependencyVersion,
        IReadOnlyList<TodoDto> Nodes,
        IReadOnlyList<TodoDependencyDto> Dependencies,
        IReadOnlyList<RoadmapNodeLayoutDto> Layouts);

    public record DevelopmentPlanDto(
        Guid UID, string Name, string? Description, bool IsArchived,
        DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, long DependencyVersion) : IEntityDto;
}
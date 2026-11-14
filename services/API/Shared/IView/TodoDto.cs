namespace AqLife.Shared.IView
{
    public record TodoDto(
        Guid UID,
        Guid PlanId,
        string Title,
        string Status,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt,
        DateTimeOffset? CompletedAt,
        int SortOrder,
        IReadOnlyCollection<Guid> PredecessorIds, // 依赖于当前任务的任务 ID 列表
        int IncompleteDependencyCount = 0,
        string? Description = null,
        bool IsBlocked = true,
        bool IsArchived = false) : IEntityDto
    { }

    public record TodoDependencyDto(Guid UID, Guid PredecessorId, Guid SuccessorId, DateTimeOffset CreatedAt):IEntityDto;
    public record RoadmapNodeLayoutDto(Guid TodoId, double X, double Y, DateTimeOffset UpdatedAt):IEntityDto;
    /// <summary>
    /// 整个开发计划的路线图 : 由若干个布局节点+todo信息 + 依赖图组合而成
    /// </summary>
    /// <param name="PlanId"></param>
    /// <param name="PlanName"></param>
    /// <param name="DependencyVersion"></param>
    /// <param name="Nodes"></param>
    /// <param name="Dependencies"></param>
    /// <param name="Layouts"></param>
    public record TodoRoadmapDto(
        Guid PlanId,
        long DependencyVersion,
        IReadOnlyList<TodoDto> Nodes,
        IReadOnlyList<TodoDependencyDto> Dependencies,
        IReadOnlyList<RoadmapNodeLayoutDto> Layouts):IEntityDto;

    public record DevelopmentPlanDto(
        Guid UID, string Name, string? Description, bool IsArchived,
        DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt) : IEntityDto;
}
using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers
{
    [Mapper]
    public sealed partial class TodoMapper :
        IViewMapper<TodoEntity, TodoDto>,
        ICreateMapper<TodoEntity, CreateTodoCommand>,
        ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand>, IViewMapper<TodoDependencyEntity, TodoDependencyDto>,
        ICreateMapper<RoadmapNodeLayoutEntity, SaveRoadmapNodeLayoutCommand>,IViewMapper<RoadmapNodeLayoutEntity, RoadmapNodeLayoutDto>,IViewMapper<DevelopmentPlanEntity, TodoRoadmapDto>
    {
        [MapProperty(nameof(TodoEntity.Status), nameof(TodoDto.Status))]
        [MapProperty(nameof(TodoEntity.IncomingDependencies), nameof(TodoDto.PredecessorIds), Use = nameof(MapToPredecessorIds))]
        [MapProperty(nameof(TodoEntity.IncomingDependencies), nameof(TodoDto.IsBlocked))]
        [MapProperty(nameof(TodoEntity.IncomingDependencies.Count), nameof(TodoDto.IncompleteDependencyCount))]
        [MapperIgnoreSource(nameof(TodoEntity.LayoutEntity))]
        [MapperIgnoreSource(nameof(TodoEntity.Plan))]
        [MapperIgnoreSource(nameof(TodoEntity.OutgoingDependencies))]
        public partial TodoDto ToDto(TodoEntity source);

        public string Convert(TodoStatus status) => status.ToString();

        private static bool IsBlocked(TodoDependencyEntity todoDependencies)
            => todoDependencies.PredecessorTodo.Status != TodoStatus.Completed;

        public bool Convert(ICollection<TodoDependencyEntity> todoDependencies)
            => todoDependencies.Any(IsBlocked); // 未完成即视为阻塞

        public int ConvertToCount(ICollection<TodoDependencyEntity> todoDependencies)
           => todoDependencies.Count(IsBlocked);

        /// <summary>
        /// 主用于 返回 incomingDependencies 的 PredecessorId
        /// </summary>
        /// <param name="incomingDependencies"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<Guid> MapToPredecessorIds(ICollection<TodoDependencyEntity> incomingDependencies)
            => [.. incomingDependencies.Select(x => x.PredecessorId)];

        public static IReadOnlyCollection<Guid> MapToSuccessorIds(ICollection<TodoDependencyEntity> outgoingDependencies)
            => [.. outgoingDependencies.Select(x => x.SuccessorId)];


        [MapperIgnoreTarget(nameof(TodoEntity.UID))]
        [MapperIgnoreTarget(nameof(TodoEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.UpdatedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.CompletedAt))]
        [MapperIgnoreTarget(nameof(TodoEntity.IsArchived))]
        [MapperIgnoreTarget(nameof(TodoEntity.Status))]
        [MapperIgnoreTarget(nameof(TodoEntity.IncomingDependencies))]
        [MapperIgnoreTarget(nameof(TodoEntity.OutgoingDependencies))]
        [MapperIgnoreTarget(nameof(TodoEntity.LayoutEntity))]
        [MapperIgnoreTarget(nameof(TodoEntity.Plan))]
        public partial TodoEntity ToEntity(CreateTodoCommand command);

        [MapperIgnoreTarget(nameof(TodoDependencyEntity.UID))]
        [MapperIgnoreTarget(nameof(TodoDependencyEntity.CreatedAt))]
        [MapperIgnoreTarget(nameof(TodoDependencyEntity.PredecessorTodo))]
        [MapperIgnoreTarget(nameof(TodoDependencyEntity.SuccessorTodo))]
        public partial TodoDependencyEntity ToEntity(AddTodoDependencyCommand command);

        [MapperIgnoreSource(nameof(TodoDependencyEntity.UID))]
        [MapperIgnoreSource(nameof(TodoDependencyEntity.PlanId))]
        [MapperIgnoreSource(nameof(TodoDependencyEntity.PredecessorTodo))]
        [MapperIgnoreSource(nameof(TodoDependencyEntity.SuccessorTodo))]
        public partial TodoDependencyDto ToDto(TodoDependencyEntity source);


        [MapProperty(nameof(SaveRoadmapNodeLayoutCommand.TodoId),nameof(RoadmapNodeLayoutEntity.UID))]
        [MapperIgnoreTarget(nameof(RoadmapNodeLayoutEntity.UpdatedAt))]
        public partial RoadmapNodeLayoutEntity ToEntity(SaveRoadmapNodeLayoutCommand command);

        [MapProperty(nameof(RoadmapNodeLayoutEntity.UID), nameof(RoadmapNodeLayoutDto.TodoId)]
        [MapperIgnoreSource(nameof(RoadmapNodeLayoutEntity.PlanId))]
        public partial RoadmapNodeLayoutDto ToDto(RoadmapNodeLayoutEntity source);

        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.Description))]
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.IsArchived))]
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.CreatedAt))]
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.UpdatedAt))]
        [MapperIgnoreSource(nameof(DevelopmentPlanEntity.Name))]
        [MapProperty(nameof(DevelopmentPlanEntity.UID),nameof(TodoRoadmapDto.PlanId))]
        [MapProperty(nameof(DevelopmentPlanEntity.Todos), nameof(TodoRoadmapDto.Nodes),Use = nameof(MapToTodoDto))]
        [MapProperty(nameof(DevelopmentPlanEntity.Todos), nameof(TodoRoadmapDto.Layouts),Use = nameof(MapToLayoutDto))]
        [MapProperty(nameof(DevelopmentPlanEntity.Todos), nameof(TodoRoadmapDto.Dependencies), Use = nameof(MapToDependencyDto))]
        public partial TodoRoadmapDto ToDto(DevelopmentPlanEntity source);


        public IReadOnlyList<TodoDto> MapToTodoDto(ICollection<TodoEntity> source) => source.Select(this.ToDto).ToList();
        public IReadOnlyList<RoadmapNodeLayoutDto> MapToLayoutDto(ICollection<TodoEntity> source) => source.Count == 0 ? [] : source.Select(e=>this.ToDto(e.LayoutEntity)).ToList();// B02 问题待定
        public IReadOnlyList<TodoDependencyDto> MapToDependencyDto(ICollection<TodoEntity> source) => source.SelectMany(e => e.IncomingDependencies.Select(this.ToDto)).ToList();
    }
}
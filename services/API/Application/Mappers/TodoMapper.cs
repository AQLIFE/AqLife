using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Services;
using AqLife.Shared.IView;

namespace AqLife.Application.Mappers
{
    public sealed class TodoMapper :
        IViewMapper<TodoEntity, TodoDto>,
        ICreateMapper<TodoEntity, CreateTodoCommand>,
        ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand>,
        ICreateMapper<RoadmapNodeLayoutEntity, SaveRoadmapNodeLayoutCommand>
    {
        public TodoDto ToDto(TodoEntity entity)
            => ToDto(entity, null, entity.Children.Select(ToDto).ToList());

        public TodoDto ToDto(
            TodoEntity entity,
            TodoDependencyState? dependencyState,
            IReadOnlyCollection<TodoDto>? children = null)
        {
            var childItems = children ?? Array.Empty<TodoDto>();
            return new TodoDto(
                entity.UID,
                entity.FTID,
                entity.Title,
                entity.Status.ToString(),
                entity.CreatedAt,
                entity.CompletedAt,
                entity.Priority,
                childItems.ToList(),
                childItems.Count > 0)
            {
                PlanId = entity.PlanId,
                Title = entity.Title,
                Description = entity.Description,
                UpdatedAt = entity.UpdatedAt,
                SortOrder = entity.SortOrder,
                IsArchived = entity.IsArchived,
                IsBlocked = dependencyState?.IsBlocked ?? false,
                IncompleteDependencyCount = dependencyState?.IncompleteDependencyCount ?? 0,
                BlockingTodoIds = dependencyState?.BlockingTodoIds ?? Array.Empty<Guid>()
            };
        }

        public TodoDependencyDto ToDto(TodoDependencyEntity entity)
            => new(entity.PredecessorId, entity.SuccessorId, entity.CreatedAt);

        public RoadmapNodeLayoutDto ToDto(RoadmapNodeLayoutEntity entity)
            => new(entity.TodoId, entity.X, entity.Y, entity.UpdatedAt);

        public TodoRoadmapDto ToRoadmapDto(
            DevelopmentPlanEntity plan,
            IReadOnlyList<TodoDto> nodes,
            IReadOnlyList<TodoDependencyDto> dependencies,
            IReadOnlyList<RoadmapNodeLayoutDto> layouts)
            => new(plan.UID, plan.Name, plan.DependencyVersion, nodes, dependencies, layouts);

        public TodoEntity ToEntity(CreateTodoCommand command)
            => new(command.PlanId, command.EffectiveTitle, command.Description, command.Priority,
                command.EffectiveParentId, command.SortOrder);

        public TodoDependencyEntity ToEntity(AddTodoDependencyCommand command)
            => new(command.PlanId, command.PredecessorId, command.SuccessorId);

        public RoadmapNodeLayoutEntity ToEntity(SaveRoadmapNodeLayoutCommand command)
            => new(command.PlanId, command.TodoId, command.X, command.Y);
    }
}
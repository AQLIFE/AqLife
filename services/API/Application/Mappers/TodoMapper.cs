using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;

namespace AqLife.Application.Business.Todo
{
    public sealed class TodoMapper : IViewMapper<TodoEntity, TodoDto>, ICreateMapper<TodoEntity, CreateTodoCommand>
    {
        public TodoDto ToDto(TodoEntity entity)
            => new(entity.UID, entity.FTID, entity.Title, entity.Status.ToString(),
                entity.CreatedAt, entity.CompletedAt, entity.Priority,
                entity.Children.Select(ToDto).ToList(), entity.Children.Count > 0)
            {
                PlanId = entity.PlanId,
                Title = entity.Title,
                Description = entity.Description,
                UpdatedAt = entity.UpdatedAt,
                SortOrder = entity.SortOrder,
                IsArchived = entity.IsArchived
            };

        public TodoEntity ToEntity(CreateTodoCommand command)
            => new(command.PlanId, command.Title, command.Description, command.Priority,
                command.ParentId, command.SortOrder);
    }
}
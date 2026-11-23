using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
namespace AqLife.Application.Plan.Todo;
public record DeleteTodoCommand(Guid UID, Guid PlanId) : IRequireValidEntity<TodoEntity>, IDeleteCommand, IRequireDevPlan;

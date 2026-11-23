using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
namespace AqLife.Application.Plan.Todo;
public record AddTodoDependencyCommand(Guid PlanId, Guid PredecessorId, Guid SuccessorId) : ICreateCommand, IRequireDependencyVersionIncrement;
public record RemoveTodoDependencyCommand(Guid UID, Guid PlanId) : IRequireValidEntity<TodoDependencyEntity>, IDeleteCommand, IRequireDependencyVersionIncrement;

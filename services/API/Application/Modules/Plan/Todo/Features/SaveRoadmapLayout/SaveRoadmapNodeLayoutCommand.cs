using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
namespace AqLife.Application.Plan.Todo;
public record SaveRoadmapNodeLayoutCommand(Guid PlanId, Guid TodoId, double X, double Y) : ICommand, IRequireDevPlan;

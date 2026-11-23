namespace AqLife.Application.Plan.Contracts;

/// <summary>
/// Marks a request that operates on a development plan and must respect its archived state.
/// </summary>
public interface IRequireDevPlan
{
    Guid PlanId { get; init; }
}

/// <summary>
/// Marks a plan request that increments the dependency graph version in the same transaction.
/// </summary>
public interface IRequireDependencyVersionIncrement : IRequireDevPlan, AqLife.Application.Abstractions.Commands.IRequireTransaction
{
}

/// <summary>
/// Marks a query that needs development-plan-specific search handling.
/// </summary>
public interface IReadDevPlan
{
}

using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.Pipeline;

/// <summary>
/// Applies Plan-specific dependency-version rules inside the surrounding transaction.
/// </summary>
public sealed class DependencyVersionIncrementBehavior<TRequest, TResponse>(
    IPlanPersistence persistence) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        if (request is IRequireDependencyVersionIncrement command)
        {
            var plan = await persistence.DevelopmentPlans.SingleAsync(
                entity => entity.UID == command.PlanId,
                cancellationToken);

            plan.IncrementDependencyVersion();
        }

        return response;
    }
}

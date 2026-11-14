using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using AqLife.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.CoreTest.IntegrationTest.Todo;

[Collection<IntegrationTestCollection>]
public class TodoDependencyValidationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase<TodoEntity>(fixture)
{
    [Fact]
    public async Task AddDependency_RejectsCycleInCurrentPlan()
    {
        var planId = Guid.NewGuid();
        var predecessorId = Guid.NewGuid();
        var successorId = Guid.NewGuid();

        try
        {
            await SeedPlanAndTodosAsync(planId, predecessorId, successorId);

            await ExecuteAsync(
                new AddTodoDependencyCommand(planId, predecessorId, successorId),
                TestContext.Current.CancellationToken);

            var exception = await Assert.ThrowsAsync<DomainLegalityException>(
                () => ExecuteAsync(
                    new AddTodoDependencyCommand(planId, successorId, predecessorId),
                    TestContext.Current.CancellationToken));

            Assert.Equal("禁止创建循环依赖", exception.Message);
        }
        finally
        {
            await RemovePlanAndTodosAsync(planId);
        }
    }

    private async Task SeedPlanAndTodosAsync(Guid planId, Guid firstId, Guid secondId)
    {
        using var scope = CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();

        var plan = new DevelopmentPlanEntity($"Dependency test {planId:N}") { UID = planId };
        var first = new TodoEntity(planId, "First") { UID = firstId };
        var second = new TodoEntity(planId, "Second") { UID = secondId };

        first.LayoutEntity = new RoadmapNodeLayoutEntity(first.UID, planId, 0, 0);
        second.LayoutEntity = new RoadmapNodeLayoutEntity(second.UID, planId, 1, 1);
        plan.Todos.Add(first);
        plan.Todos.Add(second);

        storage.DevelopmentPlans.Add(plan);
        storage.Todo.AddRange(first, second);
        await storage.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task RemovePlanAndTodosAsync(Guid planId)
    {
        using var scope = CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();

        var dependencies = await storage.TodoDependencies
            .Where(item => item.PlanId == planId)
            .ToListAsync(TestContext.Current.CancellationToken);
        var layouts = await storage.RoadmapNodeLayouts
            .Where(item => item.PlanId == planId)
            .ToListAsync(TestContext.Current.CancellationToken);
        var todos = await storage.Todo
            .Where(item => item.PlanId == planId)
            .ToListAsync(TestContext.Current.CancellationToken);
        var plan = await storage.DevelopmentPlans
            .SingleOrDefaultAsync(item => item.UID == planId, TestContext.Current.CancellationToken);

        storage.RemoveRange(dependencies);
        storage.RemoveRange(layouts);
        storage.RemoveRange(todos);
        if (plan is not null)
            storage.DevelopmentPlans.Remove(plan);

        await storage.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}

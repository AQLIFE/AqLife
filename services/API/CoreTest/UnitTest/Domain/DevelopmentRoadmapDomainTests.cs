using AqLife.Domain.Entities;
using AqLife.Domain.Services;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;

namespace AqLife.CoreTest.UnitTest.Domain
{
    public class DevelopmentRoadmapDomainTests
    {
        private static readonly Guid PlanId = Guid.NewGuid();

        [Fact]
        public void DevelopmentPlan_CannotBeArchivedTwiceWithSideEffects_AndCannotArchiveDefaultPlan()
        {
            var plan = new DevelopmentPlanEntity("  Roadmap  ");
            plan.Archive();

            Assert.True(plan.IsArchived);
            Assert.Throws<DomainLegalityException>(() => plan.Rename("Renamed"));

            var defaultPlan = new DevelopmentPlanEntity(
                DevelopmentPlanEntity.DefaultPlanId,
                "默认开发计划");

            Assert.Throws<DomainLegalityException>(() => defaultPlan.Archive());
        }

        [Fact]
        public void Todo_CompletionDoesNotChangePriority_AndReopenClearsCompletedAt()
        {
            var todo = new TodoEntity(PlanId, "Task", priority: 7);

            todo.Complete();

            Assert.Equal(TodoStatus.Completed, todo.Status);
            Assert.Equal(7, todo.Priority);
            Assert.NotNull(todo.CompletedAt);

            todo.Reopen();

            Assert.Equal(TodoStatus.Wait, todo.Status);
            Assert.Null(todo.CompletedAt);
            Assert.Equal(7, todo.Priority);
        }

        [Fact]
        public void HierarchyValidator_RejectsMovingTaskUnderItsDescendant()
        {
            var root = new TodoEntity(PlanId, "Root");
            var child = new TodoEntity(PlanId, "Child", parentId: root.UID);
            var grandchild = new TodoEntity(PlanId, "Grandchild", parentId: child.UID);
            var tasks = new[] { root, child, grandchild };

            Assert.Throws<DomainLegalityException>(
                () => TodoHierarchyValidator.EnsureCanMove(root, grandchild.UID, tasks));
        }

        [Fact]
        public void HierarchyValidator_RejectsParentFromAnotherPlan()
        {
            var task = new TodoEntity(PlanId, "Task");
            var otherPlanParent = new TodoEntity(Guid.NewGuid(), "Other plan");

            Assert.Throws<DomainLegalityException>(
                () => TodoHierarchyValidator.EnsureCanMove(task, otherPlanParent.UID, new[] { task, otherPlanParent }));
        }

        [Fact]
        public void DependencyGraphValidator_RejectsCycle()
        {
            var first = new TodoEntity(PlanId, "A");
            var second = new TodoEntity(PlanId, "B");
            var third = new TodoEntity(PlanId, "C");
            var tasks = new[] { first, second, third };
            var edges = new[]
            {
                new TodoDependencyEntity(PlanId, first.UID, second.UID),
                new TodoDependencyEntity(PlanId, second.UID, third.UID)
            };
            var candidate = new TodoDependencyEntity(PlanId, third.UID, first.UID);

            Assert.Throws<DomainLegalityException>(
                () => TodoDependencyGraphValidator.EnsureCanAdd(candidate, tasks, edges));
        }

        [Fact]
        public void DependencyGraphValidator_RejectsDuplicateEdge()
        {
            var first = new TodoEntity(PlanId, "A");
            var second = new TodoEntity(PlanId, "B");
            var tasks = new[] { first, second };
            var existing = new TodoDependencyEntity(PlanId, first.UID, second.UID);
            var duplicate = new TodoDependencyEntity(PlanId, first.UID, second.UID);

            Assert.Throws<DomainLegalityException>(
                () => TodoDependencyGraphValidator.EnsureCanAdd(duplicate, tasks, new[] { existing }));
        }
    }
}

using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
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
        public void Todo_ArchiveDoesNotOverloadPriority()
        {
            var todo = new TodoEntity(PlanId, "Deprecated task", priority: 5);

            todo.Archive();

            Assert.True(todo.IsArchived);
            Assert.Equal(5, todo.Priority);
            Assert.Throws<DomainLegalityException>(() => todo.Update("Changed", TodoStatus.Wait, 5));

            todo.Restore();

            Assert.False(todo.IsArchived);
            todo.Update("Changed", TodoStatus.Wait, 5);
            Assert.Equal("Changed", todo.Title);
        }

        [Fact]
        public void LegacyTodoConstructor_AssignsDefaultDevelopmentPlan()
        {
            var todo = new TodoEntity("Legacy task");

            Assert.Equal(DevelopmentPlanEntity.DefaultPlanId, todo.PlanId);
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
        public void DependencyGraphValidator_RejectsArchivedEndpoints()
        {
            var predecessor = new TodoEntity(PlanId, "Archived predecessor");
            var successor = new TodoEntity(PlanId, "Successor");
            predecessor.Archive();
            var candidate = new TodoDependencyEntity(PlanId, predecessor.UID, successor.UID);

            Assert.Throws<DomainLegalityException>(
                () => TodoDependencyGraphValidator.EnsureCanAdd(candidate, new[] { predecessor, successor }, Array.Empty<TodoDependencyEntity>()));
        }

        [Fact]
        public void DependencyStateCalculator_UsesAndSemantics()
        {
            var first = new TodoEntity(PlanId, "A");
            var second = new TodoEntity(PlanId, "B");
            var target = new TodoEntity(PlanId, "Target");
            var dependencies = new[]
            {
                new TodoDependencyEntity(PlanId, first.UID, target.UID),
                new TodoDependencyEntity(PlanId, second.UID, target.UID)
            };
            first.Complete();

            var state = TodoDependencyStateCalculator.Calculate(
                target,
                dependencies,
                new Dictionary<Guid, TodoEntity>
                {
                    [first.UID] = first,
                    [second.UID] = second,
                    [target.UID] = target
                });

            Assert.True(state.IsBlocked);
            Assert.Equal(1, state.IncompleteDependencyCount);
            Assert.Equal(new[] { second.UID }, state.BlockingTodoIds);
        }

        [Fact]
        public void DependencyStateCalculator_CompletedTaskIsNeverBlocked()
        {
            var predecessor = new TodoEntity(PlanId, "Predecessor");
            var target = new TodoEntity(PlanId, "Target");
            var dependency = new TodoDependencyEntity(PlanId, predecessor.UID, target.UID);
            target.Complete();

            var state = TodoDependencyStateCalculator.Calculate(
                target,
                new[] { dependency },
                new Dictionary<Guid, TodoEntity>
                {
                    [predecessor.UID] = predecessor,
                    [target.UID] = target
                });

            Assert.False(state.IsBlocked);
            Assert.Equal(0, state.IncompleteDependencyCount);
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

        [Fact]
        public void Todo_TitleAndDescription_AreNormalizedAndStoredSeparately()
        {
            var todo = new TodoEntity(PlanId, "  Build roadmap  ", "  Detailed notes  ", priority: 4);

            Assert.Equal("Build roadmap", todo.Title);
            Assert.Equal("Detailed notes", todo.Description);

            todo.UpdateDetails("  Implement API  ", "  Backend details  ", 8);

            Assert.Equal("Implement API", todo.Title);
            Assert.Equal("Backend details", todo.Description);
            Assert.Equal(8, todo.Priority);
        }

        [Fact]
        public void Todo_UpdateDetails_ValidatesBeforeChangingExistingValues()
        {
            var todo = new TodoEntity(PlanId, "Original", "Original description", priority: 3);

            Assert.Throws<DomainLegalityException>(
                () => todo.UpdateDetails("Changed", new string('x', 10001), 9));

            Assert.Equal("Original", todo.Title);
            Assert.Equal("Original description", todo.Description);
            Assert.Equal(3, todo.Priority);
        }

        [Fact]
        public void DevelopmentPlan_Update_ValidatesBeforeChangingExistingValues()
        {
            var plan = new DevelopmentPlanEntity("Original", "Original description");

            Assert.Throws<DomainLegalityException>(
                () => plan.Update("Changed", new string('x', 2001)));

            Assert.Equal("Original", plan.Name);
            Assert.Equal("Original description", plan.Description);
        }

        [Fact]
        public void TodoCommands_SupportBothNewAndLegacyPayloadShapes()
        {
            var parentId = Guid.NewGuid();
            var legacy = new CreateTodoCommand
            {
                LegacyDesc = "Legacy title",
                LegacyParentId = parentId
            };

            Assert.Equal(DevelopmentPlanEntity.DefaultPlanId, legacy.PlanId);
            Assert.Equal("Legacy title", legacy.EffectiveTitle);
            Assert.Equal(parentId, legacy.EffectiveParentId);

            var modern = new CreateTodoCommand
            {
                PlanId = PlanId,
                Title = "Modern title",
                ParentId = parentId,
                Description = "Task details"
            };

            Assert.Equal(PlanId, modern.PlanId);
            Assert.Equal("Modern title", modern.EffectiveTitle);
            Assert.Equal(parentId, modern.EffectiveParentId);
        }
    }
}

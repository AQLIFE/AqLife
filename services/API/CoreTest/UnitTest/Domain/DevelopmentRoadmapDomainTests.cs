using AqLife.Domain.Entities.DevPlan;
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
            Assert.Equal("Roadmap", plan.Name);
            plan.Archive();

            Assert.True(plan.IsArchived);
            Assert.Throws<DomainLegalityException>(() => plan.Rename("Renamed"));

            var defaultPlan = new DevelopmentPlanEntity()
            {
                UID = DevelopmentPlanEntity.DefaultPlanId
            };
            defaultPlan.Rename("默认开发计划");
            Assert.Throws<DomainLegalityException>(() => defaultPlan.Archive());
        }

        [Fact]
        public void Todo_ArchiveDoesNotOverloadSortOrder()
        {
            var todo = new TodoEntity(PlanId, "Deprecated task", sortOrder: 5);
            todo.Archive();

            Assert.True(todo.IsArchived);
            Assert.Equal(5, todo.SortOrder);
            Assert.Throws<DomainLegalityException>(() => todo.UpdateDetails("Changed", "desc", 5));

            todo.Restore();
            Assert.False(todo.IsArchived);
            todo.UpdateDetails("Changed", "desc", 5);
            Assert.Equal("Changed", todo.Title);
        }

        [Fact]
        public void LegacyTodoConstructor_AssignsDefaultDevelopmentPlan()
        {
            var todo = new TodoEntity("Legacy task");
            Assert.Equal(DevelopmentPlanEntity.DefaultPlanId, todo.PlanId);
        }

        [Fact]
        public void Todo_CompletionDoesNotChangeSortOrder_AndReopenClearsCompletedAt()
        {
            var todo = new TodoEntity(PlanId, "Task", sortOrder: 7);
            todo.Complete();

            Assert.Equal(TodoStatus.Completed, todo.Status);
            Assert.Equal(7, todo.SortOrder);
            Assert.NotNull(todo.CompletedAt);

            todo.Reopen();

            Assert.Equal(TodoStatus.Wait, todo.Status);
            Assert.Null(todo.CompletedAt);
            Assert.Equal(7, todo.SortOrder);
        }

        [Fact]
        public void TodoConstructor_TrimsTitleAndDescription()
        {
            var todo = new TodoEntity(PlanId, "  Build roadmap  ", "  Detailed notes  ", sortOrder: 4);

            Assert.Equal("Build roadmap", todo.Title);
            Assert.Equal("Detailed notes", todo.Description);
            Assert.Equal(4, todo.SortOrder);
        }

        [Fact]
        public void TodoUpdateDetails_StoresProvidedValuesWithoutConstructorNormalization()
        {
            var todo = new TodoEntity(PlanId, "Original", "Original description", sortOrder: 3);
            todo.UpdateDetails("  Implement API  ", "  Backend details  ", 8);

            Assert.Equal("  Implement API  ", todo.Title);
            Assert.Equal("  Backend details  ", todo.Description);
            Assert.Equal(8, todo.SortOrder);
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
    }
}
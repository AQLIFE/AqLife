using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;

namespace AqLife.APIUnitTest.Domain
{
    public class TodoEntityTests
    {
        [Fact]
        public void SetParent_ShouldSetParentId()
        {
            TodoEntity todo = new("测试任务1");
            var parentId = Guid.NewGuid();
            todo.SetParent(parentId);
            Assert.Equal(parentId, todo.FTID);
        }

        [Fact]
        public void SetParent_ShouldRejectEmptyGuid()
        {
            var todo = new TodoEntity("测试任务2");

            Assert.Throws<DomainLegalityException>(
                () => todo.SetParent(Guid.Empty)
            );
        }
    }
}

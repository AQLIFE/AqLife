using AqLife.Domain.Entities;

namespace AqLife.CoreTest.UnitTest.Domain
{
    public class TagEntityTests
    {
        [Fact]
        public void UpdateProfile_ShouldUpdateTag()
        {
            TagEntity tag = new("测试标签1");
            Assert.Equal("测试标签1", tag.Name);

            tag.Update("测试标签2");
            Assert.Equal("测试标签2", tag.Name);
        }
    }
}

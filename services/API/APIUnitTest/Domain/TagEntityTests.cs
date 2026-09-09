using AqLife.Domain.Entities;

namespace AqLife.APIUnitTest.Domain
{
    public class TagEntityTests
    {
        [Fact]
        public void UpdateProfile_ShouldUpdateTag()
        {
            TagEntity tag = new("测试标签1", true);
            Assert.Equal("测试标签1", tag.Name);
            Assert.True(tag.IsCategory);
            tag.CancelCategory();
            Assert.False(tag.IsCategory);
            tag.Update("测试标签2");
            Assert.Equal("测试标签2", tag.Name);
        }
    }
}

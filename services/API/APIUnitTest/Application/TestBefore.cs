using System.Reflection;
using Xunit.v3;

namespace AqLife.APIUnitTest.Application
{
    public sealed class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest, IXunitTest test)
        {
            var fixture = TestContext.Current
                .GetFixture<IntegrationTestFixture>()
                .AsTask()
                .GetAwaiter()
                .GetResult();

            if (fixture is null)
                throw new InvalidOperationException(
                    "无法获取 IntegrationTestFixture");

            fixture.ResetDatabase()
                .GetAwaiter()
                .GetResult();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.v3;

namespace APIUnitTest.Application
{
    public sealed class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest,IXunitTest test)
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

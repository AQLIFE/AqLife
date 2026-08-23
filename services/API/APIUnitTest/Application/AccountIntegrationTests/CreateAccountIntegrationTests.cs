using APIUnitTest.TestData;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Domain.Command;
using MyLife.Infrastructure;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Utils;
using System.Reflection;
using System.Security.Principal;
using Xunit.v3;


namespace APIUnitTest.Application.AccountIntegrationTests
{
    [Collection<IntegrationTestCollection>]
    public class CreateAccountIntegrationTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
    {
        [Fact, ResetDatabase]
        public async Task CreateAccount_ShouldCreateAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            var loginName = await mediator.Send(command, CancellationToken.None);

            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.LoginName == loginName, CancellationToken.None);

            var pwdHash = FastHash.GetSha256Hash(command.Pwd);

            Assert.NotNull(account);
            Assert.Equal("测试账户1", account.Name);
            Assert.Equal("测试账户描述", account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
        }

        /// <summary>
        /// 验证需求一: 正常用户名
        /// </summary>
        /// <returns></returns>
        [InlineData("1")]
        [InlineData("a")]
        [InlineData("一")]
        [InlineData("\\")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_NormalAccount(string testSource)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            // 符合规则的测试数据
            CreateAccountCommand command = new(testSource, "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            // 执行测试
            var loginName = await mediator.Send(command, CancellationToken.None);
            // 检查测试结果
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.LoginName == loginName, CancellationToken.None);
            var pwdHash = FastHash.GetSha256Hash(command.Pwd);
            Assert.NotNull(account); // 这里通过即为PASS
            Assert.Equal(testSource, account.Name);
            Assert.Equal("测试账户描述", account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
        }

        /// <summary>
        /// 验证需求二: 空用户名
        /// </summary>
        /// <returns></returns>
        /// 
        [InlineData("")]
        [InlineData("ab ")]
        [InlineData("a b c")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_EmptyNameAccount(string testSource)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            // 符合规则的测试数据
            CreateAccountCommand command = new(testSource, "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            // 执行测试
            await Assert.ThrowsAsync<RequestCheckException>(() => mediator.Send(command, CancellationToken.None));
            // 检查测试结果
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == command.Name, CancellationToken.None);
            Assert.Null(account); // 这里通过即为PASS                                  
        }
        /// <summary>
        /// 验证需求三:超长用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_LongNameAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            // 符合规则的测试数据
            string longName = "abcdefghijklmnopquvwxyz1234567890";
            CreateAccountCommand command = new(longName, "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            // 执行测试
            await Assert.ThrowsAsync<DbUpdateException>(() => mediator.Send(command, CancellationToken.None));
            // 检查测试结果
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == command.Name, CancellationToken.None);
            Assert.Null(account); // 这里通过即为PASS        
        }
        /// <summary>
        /// 验证需求六 : 拦截超长Desc
        /// </summary>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [MemberData(nameof(AccountTestData.InvalidDescData), MemberType = typeof(AccountTestData))]
        public async Task CreateAccount_LongDescAccount(string testSource)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", testSource, "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            // 执行测试
            if (testSource.Length > 255)
            {
                // 检查测试结果
                await Assert.ThrowsAsync<DbUpdateException>(() => mediator.Send(command, CancellationToken.None));
                var account = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == command.Name, CancellationToken.None);
                Assert.Null(account); // 这里通过即为PASS        
            }
            else
            {
                var loginName =await mediator.Send(command, CancellationToken.None);
                var account = await storage.Accounts.SingleOrDefaultAsync(e => e.LoginName == loginName, CancellationToken.None);
                Assert.NotNull(account);
                Assert.True(account.Desc.Length == testSource.Length);
                Assert.Equal(testSource, account.Desc);
            }
        }

        /// <summary>
        /// 验证需求四: 拦截重复注册 | 拦截重复用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_DuplicateAccountName()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();


            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            // 执行测试
            var loginName = await mediator.Send(command, CancellationToken.None);
            // 检查测试结果
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.LoginName == loginName, CancellationToken.None);
            Assert.NotNull(account); // 这里通过即为PASS
            Assert.True(account.IsValid);

            await Assert.ThrowsAsync<RequestCheckException>(() => mediator.Send(command, CancellationToken.None));

        }

        /// <summary>
        /// 验证需求五: 拦截错误密钥
        /// </summary>
        /// <param name="testSource"></param>
        /// <returns></returns>
        [InlineData("")]
        [InlineData("9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_ErrorServerKey(string testSource)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", testSource);

            // 执行测试
            await Assert.ThrowsAsync<RequestCheckException>(() => mediator.Send(command, CancellationToken.None));
            // 检查测试结果
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == command.Name, CancellationToken.None);
            Assert.Null(account); // 这里通过即为PASS
        }
    }
}

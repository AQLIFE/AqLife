using AqLife.Domain.Command;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Services;

namespace AqLife.APIUnitTest.Application.AccountIntegrationTests
{
    [Collection<IntegrationTestCollection>]
    public class UpdateAccountIntegrationTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
    {
        /// <summary>
        /// 验证需求: 更新有效账户的profile,账户名包含空格时触发异常,反之通过
        /// </summary>
        /// <param name="testSource"></param>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [InlineData("new Name")]
        [InlineData("newName")]
        public async Task UpdateAccount_UpdateProfile(string testSource)
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

            UpdateAccountProfileCommand updateCommand = new(account.UID, testSource, "随机文本");

            if (string.IsNullOrWhiteSpace(testSource) || testSource.Any(e => char.IsWhiteSpace(e)))
            {
                await Assert.ThrowsAsync<RequestCheckException>(() => mediator.Send(updateCommand, CancellationToken.None));
                var nAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == testSource, CancellationToken.None);
                Assert.Null(nAccount);
                // 预期更新失败
                var xAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == account.Name, CancellationToken.None);
                Assert.NotNull(xAccount);
            }
            else
            {
                var xLoginname = await mediator.Send(updateCommand, CancellationToken.None);
                var tAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.LoginName == xLoginname, CancellationToken.None);
                Assert.NotNull(tAccount);
                Assert.Equal(tAccount.LoginName, account.LoginName);
            }
        }
        /// <summary>
        /// 验证需求: 更新无效的GUID触发异常
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UpdateAccount_UpdateEmptyAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            UpdateAccountProfileCommand updateCommand = new(Guid.NewGuid(), "RADOM", "随机文本");

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => mediator.Send(updateCommand, CancellationToken.None));
            // 预期更新失败
            var account = await storage.Accounts.SingleOrDefaultAsync(e => e.UID == updateCommand.UID, CancellationToken.None);
            Assert.Null(account);

        }
    }
}

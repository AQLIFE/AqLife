using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Infrastructure;
using AqLife.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Services;
using System.Linq.Expressions;

namespace AqLife.CoreTest.IntegrationTest.Account
{
    public class AccountTest(IntegrationTestFixture fixture) : IntegrationTestBase<AccountEntity>(fixture)
    {
        public async Task<AccountEntity> CreateValidAccountAsync()
        {
            string key = await EnsureSystemKeyAsync();

            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            var loginName = await ExecuteAsync(command, CancellationToken.None);

            var account = await base.QueryAsync(e => e.LoginName == loginName, CancellationToken.None);
            var expectedPwdHash = FastHash.GetSha256Hash(command.Pwd);
            Assert.NotNull(account);
            Assert.Equal("测试账户1", account.Name);
            Assert.Equal("测试账户描述", account.Desc);
            Assert.Equal(expectedPwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
            return account;
        }

        public async Task<AccountEntity> CreateDisabledAccountAsync()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            // 构建测试环境
            AccountEntity account = await CreateValidAccountAsync();
            account.Disable();// 没有EF core 实体跟踪变化,需要手动保存变化
            storage.Accounts.Update(account);
            await storage.SaveChangesAsync(CancellationToken.None);

            var disabledAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == account.Name, CancellationToken.None);
            Assert.NotNull(disabledAccount);
            Assert.False(disabledAccount.IsValid);
            return disabledAccount;
        }

        public async Task<bool> IsAccountFullyRemovedAsync(Expression<Func<AccountEntity, bool>> accountQuery, Guid accountUid, CancellationToken cancellationToken = default)
        => !await AnyAsync(accountQuery, cancellationToken) && !await AnyAsync<SubscriptionEntity>(e => e.AID == accountUid, cancellationToken);

    }
}

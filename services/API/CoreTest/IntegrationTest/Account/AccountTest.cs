using AqLife.Domain.Command;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using AqLife.Shared.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.CoreTest.IntegrationTest.Account
{
    public class AccountTest(IntegrationTestFixture fixture) : IntegrationTestBase<AccountEntity>(fixture)
    {
        public async Task<AccountEntity> CreateTestAccount()
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");

            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            var loginName = await ExecuteAsync(command, CancellationToken.None);

            var account = await base.QueryAsync(e => e.LoginName == loginName,CancellationToken.None);
            var pwdHash = FastHash.GetSha256Hash(command.Pwd);
            Assert.NotNull(account);
            Assert.Equal("测试账户1", account.Name);
            Assert.Equal("测试账户描述", account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
            return account;
        }

        public async Task<AccountEntity> CreateDisableAcount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            // 构建测试环境
            AccountEntity account = await CreateTestAccount();
            account.Disable();// 没有EF core 实体跟踪变化,需要手动保存变化
            storage.Accounts.Update(account);
            await storage.SaveChangesAsync(CancellationToken.None);

            var cloudAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == account.Name, CancellationToken.None);
            Assert.NotNull(cloudAccount);
            Assert.False(cloudAccount.IsValid);
            return cloudAccount;
        }

        /// <summary>
        ///  AccountEntity 专用检查
        /// </summary>
        /// <param name="query"></param>
        /// <param name="aid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>当 AccountEntity & Subscrtions 两者都并不存在时,返回 true,反之为 false</returns>
        public async Task<bool> IsDelete(Expression<Func<AccountEntity, bool>> query,Guid aid, CancellationToken cancellationToken = default)
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            return  !await base.AnyAsync(query, cancellationToken) && !await storage.Subscription.AnyAsync(e=>e.AID == aid);
            // 当 两者都并不存在时,返回 true,反之为 false
        }
    }
}

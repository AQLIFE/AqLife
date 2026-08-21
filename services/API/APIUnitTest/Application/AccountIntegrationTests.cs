using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Application.Business.Account.Handler;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure;
using MyLife.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUnitTest.Application
{
    public class AccountIntegrationTests(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
    {

        [Fact]
        public async Task Account_ShouldBePersisted()
        {
            // Arrange
            using var scope = fixture.Services.CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var account = new AccountEntity(
                "IntegrationTest",
                "Test Account",
                "Password123"
            );

            account.Disable();
            await storage.Accounts.AddAsync(account);
            await storage.SaveChangesAsync();

            // Assert
            var result = await storage.Accounts
                .SingleOrDefaultAsync(x => x.UID == account.UID);

            Assert.NotNull(result);
            Assert.Equal("IntegrationTest", result.Name);
            // 自恢复
            storage.Accounts.Remove(account);
            await storage.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateAccount_ShouldCreateAccount() {

            using var scope = fixture.Services.CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            Assert.False(await storage.Accounts.AnyAsync(e=>e.IsValid));// 前置检查 : 应不存在账户
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            CreateAccountCommand command = new ("测试账户1", "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");

            var loginName = await mediator.Send(command);

            var account = await storage.Accounts.SingleOrDefaultAsync(e=>e.LoginName == loginName);

            var pwdHash = FastHash.GetSha256Hash(command.pwd);
            Assert.NotNull(account);
            Assert.Equal("测试账户1", account.Name);
            Assert.Equal("测试账户描述",account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
        }
    }
}

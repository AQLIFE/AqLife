using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure;
using MyLife.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.v3;

namespace APIUnitTest.Application.AccountIntegrationTests
{
    [Collection<IntegrationTestCollection>]
    public class DeleteAccountIntegrationTests(IntegrationTestFixture fixture): IntegrationTestBase(fixture)
    {
       /// <summary>
       /// 验证需求: 完整删除账户相关数据
       /// </summary>
       /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task DeleteAccount_ShouldDeleteAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            var account = await CreateTestAccount();
            Assert.NotNull(account);
            DeleteAccountCommand command = new(account.UID);
            await mediator.Send(command, CancellationToken.None);

            Assert.False(await storage.Subscription.AnyAsync(e => e.AID == account.UID, CancellationToken.None));// 账户删除后，订阅也应该被删除
            Assert.False(await storage.Accounts.AnyAsync(e => e.UID == account.UID, CancellationToken.None));// 删除后，账户应该不存在
        }
        /// <summary>
        /// 验证需求: 重复删除,捕获异常
        /// </summary>
        /// <returns></returns>
        [Fact,ResetDatabase]
        public async Task DeleteAccount_DeleteRepeatedly()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            var account = await CreateTestAccount();
            Assert.NotNull(account);
            DeleteAccountCommand command = new(account.UID);
            await mediator.Send(command, CancellationToken.None);

            Assert.False(await storage.Subscription.AnyAsync(e => e.AID == account.UID, CancellationToken.None));// 账户删除后，订阅也应该被删除
            Assert.False(await storage.Accounts.AnyAsync(e => e.UID == account.UID, CancellationToken.None));// 删除后，账户应该不存在

            // 删除成功后,再次删除,触发异常
            await Assert.ThrowsAsync<ResourceNotFoundException>(() => mediator.Send(command,CancellationToken.None));
        }
        /// <summary>
        /// 验证需求: 删除功能不受账户禁用状体影响
        /// </summary>
        /// <returns></returns>
        [Fact(Skip = "需求废弃,核心验证逻辑由DeleteAccount_ShouldDeleteAccount完成"),ResetDatabase]
        public async Task DeleteAccount_DeleteInactiveAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            // 构建测试环境
            var account = await CreateTestAccount();
            Assert.NotNull(account);
            account.Disable();// 没有EF core 实体跟踪变化,需要手动保存变化
            storage.Accounts.Update(account);
            await storage.SaveChangesAsync(CancellationToken.None);

            var cloudAccount = await storage.Accounts.SingleOrDefaultAsync(e => e.Name == account.Name,CancellationToken.None);
            Assert.NotNull(cloudAccount);
            Assert.False(cloudAccount.IsValid);


            DeleteAccountCommand command = new(account.UID);
            await mediator.Send(command, CancellationToken.None);

            Assert.False(await storage.Subscription.AnyAsync(e => e.AID == account.UID, CancellationToken.None));// 账户删除后，订阅也应该被删除
            Assert.False(await storage.Accounts.AnyAsync(e => e.UID == account.UID, CancellationToken.None));// 删除后，账户应该不存在
        }

        private async Task<AccountEntity> CreateTestAccount()
        {
            using var scope = CreateScope();

            var storage = scope.ServiceProvider
                .GetRequiredService<AppStorage>();

            var mediator = scope.ServiceProvider
                .GetRequiredService<IMediator>();

            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", "8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92");
            
            var loginName = await mediator.Send(command, CancellationToken.None);

            return await storage.Accounts.SingleAsync(e => e.LoginName == loginName);
        }
    }
}

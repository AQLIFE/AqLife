using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;

namespace AqLife.CoreTest.IntegrationTest.Account
{
    [Collection<IntegrationTestCollection>]
    public class DeleteAccount(IntegrationTestFixture fixture) : AccountTest(fixture)
    {
        /// <summary>
        /// 验证需求: 完整删除账户相关数据
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task DeleteAccount_ShouldRemoveAccountAndSubscription()
        {
            AccountEntity account = await CreateValidAccountAsync();

            DeleteAccountCommand command = new(account.UID);
            await base.ExecuteAsync(command, CancellationToken.None);

            bool isFullyRemoved = await IsAccountFullyRemovedAsync(e => e.UID == account.UID, account.UID, CancellationToken.None);

            Assert.True(isFullyRemoved);// 账户删除后，订阅也应该被删除            
        }
        /// <summary>
        /// 验证需求: 重复删除,捕获异常
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task DeleteAccount_RepeatedDelete_ShouldThrowResourceNotFound()
        {
            AccountEntity account = await CreateValidAccountAsync();

            DeleteAccountCommand command = new(account.UID);
            await base.ExecuteAsync(command, CancellationToken.None);
            bool isFullyRemoved = await IsAccountFullyRemovedAsync(e => e.UID == account.UID, account.UID, CancellationToken.None);

            Assert.True(isFullyRemoved);// 账户删除后，订阅也应该被删除        

            // 删除成功后,再次删除,触发异常
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await base.ExecuteAsync(command, CancellationToken.None));
            Assert.Equal("资源不存在", exception.Message);
        }
        ///// <summary>
        ///// 验证需求: 删除功能不受账户禁用状体影响
        ///// </summary>
        ///// <returns></returns>
        //[Fact(Skip = "需求废弃,核心验证逻辑由DeleteAccount_ShouldDeleteAccount完成"), ResetDatabase]
        //public async Task DeleteAccount_DeleteInactiveAccount()
        //{
        //    AccountEntity account = await base.CreateDisabledAccountAsync();

        //    DeleteAccountCommand command = new(account.UID);
        //    await base.ExecuteAsync(command, CancellationToken.None);
        //    bool isFullyRemoved = await IsAccountFullyRemovedAsync(e => e.UID == account.UID, account.UID, CancellationToken.None);

        //    Assert.True(isFullyRemoved);// 账户删除后，订阅也应该被删除        
        //}


    }
}

using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;

namespace AqLife.CoreTest.IntegrationTest.Account
{
    [Collection<IntegrationTestCollection>]
    public class UpdateAccount(IntegrationTestFixture fixture) : AccountTest(fixture)
    {
        /// <summary>
        /// 验证需求: 更新有效账户的profile,账户名包含空格时触发异常
        /// </summary>
        /// <param name="testSource"></param>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UpdateAccount_EmptyName_ShouldReject()
        {
            AccountEntity account = await base.CreateValidAccountAsync();

            UpdateAccountProfileCommand updateCommand = new(account.UID, string.Empty, "随机文本");
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await base.ExecuteAsync(updateCommand, CancellationToken.None));
            Assert.Equal("账户名称不允许存在空格", exception.Message);

            var accountWithNewName = await QueryAsync(e => e.Name == updateCommand.Name, CancellationToken.None);
            Assert.Null(accountWithNewName);
            // 改为有意义的验证: 原账户名称未被修改
            var unchangedAccount = await QueryAsync(e => e.UID == account.UID, CancellationToken.None);
            Assert.NotNull(unchangedAccount);
            Assert.Equal(account.Name, unchangedAccount.Name);

        }
        /// <summary>
        /// 验证需求:更新有效账户profile成功,,账户名不含空格
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UpdateAccount_NewName_ShouldSucceed()
        {
            AccountEntity account = await base.CreateValidAccountAsync();

            UpdateAccountProfileCommand updateCommand = new(account.UID, "newName", "随机文本");
            var xLoginname = await ExecuteAsync(updateCommand, CancellationToken.None);
            var updatedAccount = await base.QueryAsync(e => e.LoginName == xLoginname, CancellationToken.None);
            Assert.NotNull(updatedAccount);
            Assert.Equal(account.LoginName, updatedAccount.LoginName);
        }


        /// <summary>
        /// 验证需求: 更新无效的GUID触发异常
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UpdateAccount_NonexistentAccount_ShouldThrow()
        {
            Guid nonexistentUid = Guid.NewGuid();
            UpdateAccountProfileCommand updateCommand = new(nonexistentUid, "RADOM", "随机文本");

            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await base.ExecuteAsync(updateCommand, CancellationToken.None));
            // 预期更新失败
            Assert.Equal("资源不存在", exception.Message);
            var account = await base.QueryAsync(e => e.UID == updateCommand.UID, CancellationToken.None);
            Assert.Null(account);

        }
    }
}

using AqLife.CoreTest.IntegrationTest;
using AqLife.Domain.Command;
using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Services;
using System.Security.Principal;

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
        public async Task UpdateAccount_UpdateProfile()
        {
            AccountEntity account = await base.CreateTestAccount();

            UpdateAccountProfileCommand updateCommand = new(account.UID, string.Empty, "随机文本");
            var exception =await Assert.ThrowsAsync<RequestCheckException>(async () =>await base.ExecuteAsync(updateCommand, CancellationToken.None));
            Assert.Equal("账户名称不允许存在空格", exception.Message);
            var nAccount = await base.QueryAsync(e => e.Name == updateCommand.Name,CancellationToken.None);
            Assert.Null(nAccount);
            Assert.NotEqual(account.Name, nAccount?.Name);
        }
        /// <summary>
        /// 验证需求:更新有效账户profile成功,,账户名不含空格
        /// </summary>
        /// <returns></returns>
        [Fact,ResetDatabase]
        public async Task UpdateAccount_ShouldNotEmptyName()
        {
            AccountEntity account = await base.CreateTestAccount();

            UpdateAccountProfileCommand updateCommand = new(account.UID,"newName", "随机文本");
            var xLoginname = await ExecuteAsync(updateCommand, CancellationToken.None);
            var tAccount = await base.QueryAsync(e => e.LoginName == xLoginname, CancellationToken.None);
            Assert.NotNull(tAccount);
            Assert.Equal(tAccount.LoginName, account.LoginName);
        }


        /// <summary>
        /// 验证需求: 更新无效的GUID触发异常
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task UpdateAccount_UpdateEmptyAccount()
        {
            UpdateAccountProfileCommand updateCommand = new(Guid.NewGuid(), "RADOM", "随机文本");

            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await base.ExecuteAsync(updateCommand, CancellationToken.None));
            // 预期更新失败
            Assert.Equal("资源不存在", exception.Message);
            var account = await base.QueryAsync(e => e.UID == updateCommand.UID, CancellationToken.None);
            Assert.Null(account);

        }
    }
}

using MyLife.Domain.Entities;
using MyLife.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUnitTest.Domain
{
    public class AccountEntityTests
    {
        /// <summary>
        /// Account 需求 : 更新账户信息成功
        /// 预期: 更新成功
        /// </summary>
        [Fact]
        public void UpdateProfile_ShouldUpdateNameAndDescription()
        {
            var account = new AccountEntity(
                "OldName",
                "OldDesc",
                "Password123"
                );

            // Act
            account.UpdateProfile(
                "NewName",
                "NewDesc"
            );

            // Assert
            Assert.Equal("NewName", account.Name);
            Assert.Equal("NewDesc", account.Desc);
        }
        /// <summary>
        /// Account 需求 : 更新账户密码成功
        /// 预期 : 更新成功
        /// </summary>
        [Fact]
        public void ChangePassword_ShouldHashPassword()
        {
            AccountEntity account = new(
                "Test",
                "OldPassword123"
                );

            account.ChangePassword("NewPassword123");

            Assert.Equal(
                FastHash.GetSha256Hash("NewPassword123"),
                account.LoginPasswordHash);
        }
        /// <summary>
        /// Account 需求 : 更新账户头像成功
        /// 预期: 更新成功
        /// </summary>
        [Fact]
        public void ChangeAvatar_ShouldAvatar()
        {
            AccountEntity account = new("test", "test...", "oldPassword");
            Guid temp = Guid.NewGuid();
            account.ChangeAvatar(temp);
            Assert.Equal(temp, account.Avatar);
        }
        /// <summary>
        ///  Account 需求 : 更新 订阅列表成功
        ///  预期: 更新成功且数量一致
        /// </summary>
        [Fact]
        public void ReplaceSubscriptions_ShouldReplaceSubscriptions()
        {
            AccountEntity account = new("test", "test...", "oldPassword");
            List<SubscriptionEntity> oldSubscriptions = [
                new(account.UID, "测试平台web链接", "测试订阅平台1", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "测试订阅平台2", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "测试订阅平台3", "测试平台账户名", Guid.NewGuid())];
            account.ReplaceSubscriptions(oldSubscriptions);

            Assert.Equal(oldSubscriptions, account.Subscriptions);
            Assert.Equal(oldSubscriptions.Count, account.Subscriptions.Count);
            Assert.All(oldSubscriptions, e =>
            {
                Assert.Contains(e, account.Subscriptions);
                Assert.Equal(account.UID, e.AID);
                Assert.Same(account,e.Account);
            });

            List<SubscriptionEntity> newSubscriptions = [
                new(account.UID, "测试平台web链接", "公共订阅平台1", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "测试订阅平台2", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "测试订阅平台3", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "公共订阅平台4", "测试平台账户名", Guid.NewGuid()),
                new(account.UID, "测试平台web链接", "公共订阅平台5", "测试平台账户名", Guid.NewGuid()),
                ];

            account.ReplaceSubscriptions(newSubscriptions);
            Assert.All(newSubscriptions, e =>
            {
                Assert.Contains(e, account.Subscriptions);
                Assert.Equal(account.UID, e.AID);
                Assert.Same(account, e.Account);
            });


        }
    }
}

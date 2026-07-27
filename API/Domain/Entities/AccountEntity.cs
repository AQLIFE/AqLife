using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Contracts;
using MyLife.Shared.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Domain.Entities
{
    [Table("Subscriptions"), Index(nameof(SubscriptionPlatform), nameof(SubscriptionLink), IsUnique = true)]
    public class SubscriptionEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid AID { get; set; }
        [StringLength(16, ErrorMessage = "订阅账户名限制")]
        public string AliasName { get; set; } = String.Empty;
        public required string SubscriptionLink { get; set; }
        public required string SubscriptionPlatform { get; set; }
        public Guid? SubscriptionIcon { get; set; }

        [ForeignKey(nameof(AID))]
        public virtual AccountEntity Account { get; set; } = null!;
    }

    [Table("Accounts"), Index(nameof(Name), IsUnique = true)]
    [Index(nameof(LoginName), IsUnique = true)]
    public class AccountEntity : IUserEntity, IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();
        [StringLength(32), Column]
        public string Name { get;private set; } = "Demo";
        [StringLength(255),Column]
        public string? Desc { get;private set; }

        /// <summary>
        /// 使用:设计 决定 Filecontroller 必须返回对应头像GUID
        /// </summary>
        [Column]
        public Guid? Avatar { get; private set; }
        [Column]
        public bool IsValid { get;private set; } = true;

        [Column]
        public string LoginName { get;init; } = IdentityGenerator.GenerateSecureLoginName();
        [Column]
        public string LoginPasswordHash { get; private set; } = string.Empty;

        public virtual ICollection<SubscriptionEntity> Subscriptions { get; set; } = [];

        public AccountEntity() { }
        public AccountEntity(string name, string? desc, string pwd)
        {
            Name = name;
            Desc = desc;
            LoginPasswordHash = FastHash.GetSha256Hash(pwd);
        }

        public void UpdateProfile(string name, string? desc)
        {
            Name = name;
            Desc = desc;
        }
        public void ChangeAvatar(Guid guid)
        {
            Avatar = guid;
        }
        public void ReplaceSubscriptions(IEnumerable<SubscriptionEntity> subscriptions)
        {
            Subscriptions.Clear();

            foreach (var subscription in subscriptions)
            {
                subscription.Account = this;
                subscription.AID = UID;

                Subscriptions.Add(subscription);
            }
        }

        public void ChangePassword(string pwd)
        {
            LoginPasswordHash = FastHash.GetSha256Hash(pwd);
        }
        public void Disable()
        {
            IsValid = false;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Contracts;
using MyLife.Shared.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace MyLife.Domain.Entities
{
    [Table("Subscriptions"), Index(nameof(SubscriptionPlatform), nameof(SubscriptionLink), IsUnique = true)]
    public class SubscriptionEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid AID { get;  private set; }
        [StringLength(16, ErrorMessage = "订阅账户名限制")]
        public string AliasName { get; init; } = String.Empty;
        public  string SubscriptionLink { get; init; } = String.Empty;
        public  string SubscriptionPlatform { get; init; } = String.Empty;
        public Guid SubscriptionIcon { get; init; } = Guid.Empty;

        [ForeignKey(nameof(AID))]
        public virtual AccountEntity Account { get; set; } = null!;
        public SubscriptionEntity() { }

        public SubscriptionEntity(Guid aid,string slink,string sPlatform,string sName,Guid icon)
        {
            AID = aid;
            AliasName = sName;
            SubscriptionLink = slink;
            SubscriptionPlatform = sPlatform;
            SubscriptionIcon = icon;
        }

        public void BindAccount(AccountEntity account)
        {
            AID = account.UID;
            Account  = account;
        }
    }

    [Table("Accounts"), Index(nameof(Name), IsUnique = true)]
    [Index(nameof(LoginName), IsUnique = true)]
    public class AccountEntity : IUserEntity, IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();
        [StringLength(32,ErrorMessage = "账户名称长度必须介于32之内"), Column]
        public string Name { get; private set; } = string.Empty;
        [StringLength(255,ErrorMessage ="最大允许255个字符"), Column]
        public string Desc { get; private set; } = string.Empty;

        /// <summary>
        /// 使用:设计 决定 Filecontroller 必须返回对应头像GUID
        /// </summary>
        [Column]
        public Guid Avatar { get; private set; } = Guid.Empty;
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
            if(desc is not null)Desc = desc;
            LoginPasswordHash = FastHash.GetSha256Hash(pwd);
        }
        public AccountEntity(string name, string pwd)
        {
            Name = name;
            LoginPasswordHash = FastHash.GetSha256Hash(pwd);
        }

        public void UpdateProfile(string name, string desc)
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
                subscription.BindAccount(this);

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

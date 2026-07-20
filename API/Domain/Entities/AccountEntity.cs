using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Contracts;
using MyLife.Shared.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Domain.Entities
{
    [Table("Subscriptions"), Index(nameof(SubscriptionPlatform), nameof(SubscriptionLink),IsUnique =true)]
    public class SubscriptionEntity : IEntity
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();

        public Guid AID { get; set; }
        [StringLength(16,ErrorMessage ="订阅账户名限制")]
        public string AliasName { get; set; } = String.Empty;
        public required string SubscriptionLink { get; set; }
        public required string SubscriptionPlatform { get; set; }
        public Guid? SubscriptionIcon { get; set; }

        [ForeignKey(nameof(AID))]
        public virtual AccountEntity Account { get; set; } = null!;
    }

    [Table("Accounts"), Index(nameof(Name),IsUnique =true)]
    [Index(nameof(LoginName),IsUnique = true)]
    public class AccountEntity : IUserEntity, IEntity
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();
        [StringLength(10), Column]
        public string Name { get; set; } = "Demo";
        [Column]
        public string? Desc { get; set; }

        /// <summary>
        /// 使用:设计 决定 Filecontroller 必须返回对应头像GUID
        /// </summary>
        [Column]
        public Guid? Avatar { get; set; }
        [Column]
        public bool IsValid { get; set; } = true;

        [Column]
        public string LoginName { set; get; } = IdentityGenerator.GenerateSecureLoginName();
        [Column]
        public string LoginPasswordHash { set; get; } = string.Empty;

        public virtual ICollection<SubscriptionEntity> Subscriptions { get; set; } = [];

        public AccountEntity() { }
        public AccountEntity(string name,string? desc,string pwd)
        {
            Name = name;
            Desc = desc;
            LoginPasswordHash = FastHash.GetSha256Hash(pwd);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MyLife.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLife.Data.Entities
{
    [Table("Subscriptions"), Index(nameof(SubscriptionPlatform),nameof(SubscriptionLink))]
    public class SubscriptionEntity: IStorageEntity
    {
        [Key]
        public Guid SID { get; set; } = Guid.NewGuid();

        [ForeignKey("UID")]
        public Guid UID { get; set; }
        [StringLength(16)]
        public string AliasName { get; set; } = String.Empty;
        public required string SubscriptionLink { get; set; }
        public required string SubscriptionPlatform { get; set; }
        public Guid? SubscriptionIcon { get; set; }

        [ForeignKey(nameof(UID))]
        public virtual AccountEntity Account { get; set; } = null!;
    }

    [Table("Accounts"),Index(nameof(Name))]
    public class AccountEntity : IBaseUser,IStorageEntity
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
        public Guid? Avatar { get;set;}
        [Column]
        public bool IsValid { get; set; } = true;
        public virtual ICollection<SubscriptionEntity> Subscriptions { get; set; } = [];
    }
}

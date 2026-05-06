using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Data.Entities
{
    [Table("Subscriptions")]
    public class SubscriptionEntity
    {
        [Key]
        public Guid SID { get; set; }= Guid.NewGuid();
        
        [ForeignKey("UID")]
        public Guid UID { get; set; }
        [StringLength(16)]
        public string AliasName { get; set; } = String.Empty;
        public required string SubscriptionLink { get; set; }
        public required string SubscriptionPlatform { get; set; }
        public required string SubscriptionIcon { get; set; }

        [ForeignKey(nameof(UID))]
        public virtual AccountEntity Account { get; set; } = null!;
    }

    [Table("Accounts")]
    public class AccountEntity
    {
        [Key]
        public Guid UID { get; set; } = Guid.NewGuid();
        [StringLength(10),Column]
        public string Name { get; set; } = "Demo";
        public bool IsValid { get; set; } = false;
        public virtual ICollection<SubscriptionEntity> Subscriptions { get; set; } = [];
    }
}

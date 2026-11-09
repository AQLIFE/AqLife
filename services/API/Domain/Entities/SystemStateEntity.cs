using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("SystemState")]
    public class SystemStateEntity
    {
        [Key]
        public int UID { get; init; } = 1;

        public string SystemKeyHash { get; private set; } = string.Empty;

        public bool IsInitialized { get; private set; } = false;

        public SystemStateEntity(string systemKeyHash)
        {
            SystemKeyHash = systemKeyHash;
        }

        public void Initialize()
        {
            IsInitialized = true;
            SystemKeyHash = string.Empty;
        }
    }
}

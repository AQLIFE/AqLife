using AqLife.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

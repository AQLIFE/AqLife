using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Data
{
    public interface IStorageEntity
    {
        public Guid UID { get; set; }
    }
}

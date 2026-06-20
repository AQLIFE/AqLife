using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared
{
    public interface IEntity
    {
        public Guid UID { get; set; }
    }
}

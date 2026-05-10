using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared
{
    public interface IBaseUser
    {
        Guid UID { get; }
        string Name { get; }
        // 可以根据需要扩展 Role 等字段
    }
}

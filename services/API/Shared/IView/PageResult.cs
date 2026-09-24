using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Shared.IView
{
    public record PageResult<TData>(List<TData> Items,int Page,int PageSize,bool HasMore=true);
}

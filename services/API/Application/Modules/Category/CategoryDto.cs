using AqLife.Shared.IView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Category
{
    public record CategoryDto(Guid Uid, string CategoryName, string CategoryDescription, int Count = 0, Guid? ParentId = null, IEnumerable<Guid>? ResultIds = null) : IEntityDto;
}

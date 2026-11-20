using AqLife.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Abstractions.Pipeline.Markers
{
    /// <summary>
    /// 标识接口, 自动检查对应Entity 的UID是否存在
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRequireValidEntity<out TEntity> where TEntity : IEntity
    {
        Guid UID { init; get; }
    }
}

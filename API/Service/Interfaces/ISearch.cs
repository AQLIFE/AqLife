using MyLife.Domain.CommandInterface;
using MyLife.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Interfaces
{
    public interface ISearch<TQuery, TEntity, TEntityDto>
    where TQuery : IQuery<IEnumerable<TEntityDto>>
    where TEntity : IEntity
    {
        Task<IEnumerable<TEntity>> SearchAsync(TQuery query, CancellationToken ct);
    }
}

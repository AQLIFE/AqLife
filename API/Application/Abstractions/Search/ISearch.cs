using MyLife.Domain.CommandInterface;
using MyLife.Domain.Contracts;

namespace MyLife.Application.Abstractions.Search
{
    public interface ISearch<TQuery, TEntity, TEntityDto>
    where TQuery : IQuery<IEnumerable<TEntityDto>>
    where TEntity : IEntity
    {
        Task<IEnumerable<TEntity>> SearchAsync(TQuery query, CancellationToken ct);
    }
}

namespace AqLife.Application.Abstractions.Persistence;
public interface IEntityExistenceChecker<TEntity> where TEntity : class, AqLife.Shared.Contracts.IEntity
{
    Task<bool> ExistsAsync(Guid uid, CancellationToken cancellationToken = default);
    Task<bool> AllExistAsync(IEnumerable<Guid> uids, CancellationToken cancellationToken = default);
}

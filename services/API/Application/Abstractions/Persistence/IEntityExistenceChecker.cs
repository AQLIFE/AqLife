namespace AqLife.Application.Abstractions.Persistence;
public interface IEntityExistenceChecker<TEntity> where TEntity : AqLife.Shared.Contracts.IEntity
{
    Task<bool> ExistsAsync(Guid uid, CancellationToken cancellationToken = default);
}

using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure.Persistence;

public sealed class EfEntityExistenceChecker<TEntity>(AppStorage dbContext) : IEntityExistenceChecker<TEntity>
    where TEntity : class, IEntity
{
    public Task<bool> ExistsAsync(Guid uid, CancellationToken cancellationToken = default)
        => dbContext.Set<TEntity>().AnyAsync(entity => entity.UID == uid, cancellationToken);

    public async Task<bool> AllExistAsync(IEnumerable<Guid> uids, CancellationToken cancellationToken = default)
    {
        var ids = uids.Distinct().ToArray();
        if (ids.Length == 0)
            return true;

        var count = await dbContext.Set<TEntity>()
            .CountAsync(entity => ids.Contains(entity.UID), cancellationToken);
        return count == ids.Length;
    }
}

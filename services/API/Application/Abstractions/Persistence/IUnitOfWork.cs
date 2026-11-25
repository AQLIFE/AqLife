namespace AqLife.Application.Abstractions.Persistence;

/// <summary>
/// Application-level unit of work. Exposes transaction and persistence commit operations,
/// without exposing entity sets or query access.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

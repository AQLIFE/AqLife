namespace AqLife.Infrastructure
{
    using AqLife.Application.Abstractions.Persistence;
    using Microsoft.EntityFrameworkCore.Storage;

    public sealed class EfTransaction(
        IDbContextTransaction transaction)
        : ITransaction
    {

        public Task CommitAsync(
            CancellationToken ct = default)
        {
            return transaction.CommitAsync(ct);
        }


        public Task RollbackAsync(
            CancellationToken ct = default)
        {
            return transaction.RollbackAsync(ct);
        }


        public ValueTask DisposeAsync()
        {
            return transaction.DisposeAsync();
        }
    }
}

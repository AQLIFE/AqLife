using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Infrastructure
{
    using Microsoft.EntityFrameworkCore.Storage;
    using MyLife.Application.Abstractions.Persistence;

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

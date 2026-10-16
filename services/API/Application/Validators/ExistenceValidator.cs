using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.CommandInterface;
using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Validators
{
    public class ExistenceValidator<TRequest,TEntity>(IApplicationDbContext storage) : AbstractValidator<TRequest>
        where TRequest : IRequireValidEntity<TEntity>
        where TEntity : class, IEntity
    {
        private protected override string ErrorMessage { init; get; } = "资源不存在";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.Set<TEntity>().AnyAsync(e => e.UID == query.UID, ct);

        private protected override Exception CreateException(string message)
        => new ResourceNotFoundException(message);
    }
}

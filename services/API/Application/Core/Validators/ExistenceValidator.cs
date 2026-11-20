using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Core.Validators
{
    public class ExistenceValidator<TRequest, TEntity>(IApplicationDbContext storage) : AbstractValidator<TRequest>
        where TRequest : IRequireValidEntity<TEntity>
        where TEntity : class, IEntity
    {
        protected override string ErrorMessage { init; get; } = "资源不存在";
        protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.Set<TEntity>().AnyAsync(e => e.UID == query.UID, ct);

        protected override Exception CreateException(string message)
        => new ResourceNotFoundException(message);
    }
}

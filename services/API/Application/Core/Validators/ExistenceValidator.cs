using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;

namespace Core.Validators
{
    public class ExistenceValidator<TRequest, TEntity>(IEntityExistenceChecker<TEntity> existenceChecker) : AbstractValidator<TRequest>
        where TRequest : IRequireValidEntity<TEntity>
        where TEntity : class, IEntity
    {
        protected override string ErrorMessage { init; get; } = "资源不存在";
        protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await existenceChecker.ExistsAsync(query.UID, ct);

        protected override Exception CreateException(string message)
        => new ResourceNotFoundException(message);
    }
}

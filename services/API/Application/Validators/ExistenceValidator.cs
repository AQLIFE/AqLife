using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.CommandInterface;
using AqLife.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Validators
{
    public class ExistenceValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest> where TRequest : IRequireValidEntity<IEntity>
    {
        private protected override string ErrorMessage { init; get; } = "资源不存在";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.Set<IEntity>().AnyAsync(e => e.UID == query.UID, ct);
    }
}

using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Domain.CommandInterface;
using MyLife.Domain.Contracts;

namespace MyLife.Application.Validators
{
    public class ExistenceValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest> where TRequest : IRequireValidEntity<IEntity>
    {
        private protected override string ErrorMessage { init; get; } = "资源不存在";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.Set<IEntity>().AnyAsync(e => e.UID == query.UID, ct);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Validators
{
    public class ExistenceValidator<TRequest>(AppStorage storage) : AbstractValidator<TRequest> where TRequest : IAQRequest,IMustCheckExistence<IEntity>
    {
        private protected override string ErrorMessage { init; get; } = "资源不存在";
        private protected override async Task<bool> IsValidAsync(TRequest query, CancellationToken ct)
            => await storage.Set<IEntity>().AnyAsync(e => e.UID == query.UID, ct);
    }
}

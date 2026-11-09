using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Validator
{
    public class FileQueryValidator(IApplicationDbContext dbContext) : AbstractValidator<FileQuery>
    {
        private protected override string ErrorMessage { init; get; } = "无效的搜索条件";
        private protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
        {
            if (query.CategoryUID is not Guid categoryId)
                return true;

            return await dbContext.Categories.AnyAsync(e => e.UID == categoryId, ct);
        }
    }

    public class FileQueryScopeValidator(IHttpContextAccessor httpContext) : AbstractValidator<FileQuery>
    {
        private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        private protected override string ErrorMessage { init; get; } = "无权搜索";
        private protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
        => !((query.Scope == FileScope.Template || query.Scope == FileScope.All) && !IsValid);

        private protected override Exception CreateException(string message)
        => new ForbiddenException(this.ErrorMessage);
    }
}

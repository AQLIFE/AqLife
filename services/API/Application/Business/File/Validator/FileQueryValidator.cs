using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Validator
{
    public class FileQueryValidator(IHttpContextAccessor httpContext) : AbstractValidator<FileOverviewQuery>
    {
        private protected override string ErrorMessage { init; get; } = "无权搜索详细信息";
        private protected override async Task<bool> IsValidAsync(FileOverviewQuery query, CancellationToken ct)
        => query.PublishStatus == PublishStatusSearch.Default ||(httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false);
    }

    public class FileQueryScopeValidator(IHttpContextAccessor httpContext) : AbstractValidator<FileQuery>
    {
        private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        private protected override string ErrorMessage { init; get; } = "无权搜索模板文件或全部文件";
        private protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
        => !((query.Scope == FileScope.Template || query.Scope == FileScope.All) && !IsValid);

        private protected override Exception CreateException(string message)
        => new ForbiddenException(this.ErrorMessage);
    }
}

using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Core.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.File.Validator
{
    public class FileQueryValidator(IHttpContextAccessor httpContext) : AbstractValidator<FileOverviewQuery>
    {
        protected override string ErrorMessage { init; get; } = "无权搜索详细信息";
        protected override async Task<bool> IsValidAsync(FileOverviewQuery query, CancellationToken ct)
        => query.PublishStatus == PublishStatusSearch.Default ||(httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false);
    }

    public class FileQueryScopeValidator(IHttpContextAccessor httpContext) : AbstractValidator<FileQuery>
    {
        bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        protected override string ErrorMessage { init; get; } = "无权搜索模板文件或全部文件";
        protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
        => !((query.Scope == FileScope.Template || query.Scope == FileScope.All) && !IsValid);

        protected override Exception CreateException(string message)
        => new ForbiddenException(this.ErrorMessage);
    }
}

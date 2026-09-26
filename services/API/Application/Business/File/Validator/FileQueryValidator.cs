using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;

namespace AqLife.Application.Business.File.Validator
{
    public class FileQueryValidator(IApplicationDbContext dbContext) : AbstractValidator<FileQuery>
    {
        private protected override string ErrorMessage { init; get; } = "无效的搜索条件";
        private protected override async Task<bool> IsValidAsync(FileQuery query, CancellationToken ct)
        {
            if (query.CategoryUID is not null)
                return dbContext.Tags.Any(e => e.UID == query.CategoryUID && e.IsCategory);
            else return true;
        }
        // 启用Tag 搜索后,UID 必须提供,且该Tag 必须存在且是Category
    }
}

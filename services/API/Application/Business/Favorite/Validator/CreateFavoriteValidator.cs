using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite.Validator
{
    public class CreateValidTagsFavoriteValidator(IApplicationDbContext dbContext) : AbstractValidator<CreateFavoriteCommand>
    {
        private protected override string ErrorMessage { init; get; } = "一个或多个关联标签不存在";
        private protected override async Task<bool> IsValidAsync(CreateFavoriteCommand command, CancellationToken ct)
        {
            var tagIds = (command.TagIds ?? [])
             .Where(id => id != Guid.Empty)
             .Distinct()
             .ToArray();

            if (tagIds.Length == 0)
                return true;

            var count = await dbContext.Tags
                .CountAsync(e => tagIds.Contains(e.UID), ct);

            return count == tagIds.Length;
            // 如果command.TagIds为null,则返回true,表示没有关联标签,不需要验证
            // 如果command.TagIds不为null,则验证每个标签是否存在于数据库中,如果有一个标签不存在,则返回false
        }
    }

    public class CreateValidMediasFavoriteValidator(IApplicationDbContext dbContext) : AbstractValidator<CreateFavoriteCommand>
    {
        private protected override string ErrorMessage { init; get; } = "一个或多个关联文件不存在";
        private protected override async Task<bool> IsValidAsync(CreateFavoriteCommand command, CancellationToken ct)
        {
            var fileIds = (command.Media ?? [])
            .Select(media => media.FileId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

            if (fileIds.Length == 0)
                return true;

            var count = await dbContext.File
                .CountAsync(e => fileIds.Contains(e.UID), ct);

            return count == fileIds.Length;
            // 如果command.Media为null,则返回true,表示没有关联文件,不需要验证
            // 如果command.Media不为null,则验证每个文件是否存在于数据库中,如果有一个文件不存在,则返回false
        }
    }
}

using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Favorite;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite.Validator
{
    public class CreateValidTagsFavoriteValidator(IApplicationDbContext dbContext)
        : AbstractValidator<CreateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "一个或多个关联标签不存在";

        protected override async Task<bool> IsValidAsync(CreateFavoriteCommand command, CancellationToken ct)
        {
            var tagIds = (command.TagIds ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (tagIds.Length == 0)
                return true;

            var count = await dbContext.Tags.CountAsync(e => tagIds.Contains(e.UID), ct);
            return count == tagIds.Length;
        }
    }

    public class CreateValidMediasFavoriteValidator(IApplicationDbContext dbContext)
        : AbstractValidator<CreateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "一个或多个关联文件不存在";

        protected override async Task<bool> IsValidAsync(CreateFavoriteCommand command, CancellationToken ct)
        {
            var fileIds = (command.Media ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray();

            if (fileIds.Length == 0)
                return true;

            var count = await dbContext.File.CountAsync(e => fileIds.Contains(e.UID), ct);
            return count == fileIds.Length;
        }
    }

    public class CreateValidCategoryFavoriteValidator(IApplicationDbContext dbContext)
        : AbstractValidator<CreateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "Favorite 必须指定一个有效分类";

        protected override async Task<bool> IsValidAsync(CreateFavoriteCommand command, CancellationToken ct)
            => command.CID is Guid categoryId &&
               await dbContext.Categories.AnyAsync(category => category.UID == categoryId, ct);
    }

}

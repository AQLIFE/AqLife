using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Favorite;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using Core.Validators;

namespace AqLife.Application.Favorite.Validator
{
    public class UpdateValidTagsFavoriteValidator(IEntityExistenceChecker<TagEntity> tags)
        : AbstractValidator<UpdateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "一个或多个关联标签不存在";
        protected override async Task<bool> IsValidAsync(UpdateFavoriteCommand command, CancellationToken ct)
        {
            var ids = (command.TagIds ?? []).Where(id => id != Guid.Empty).Distinct().ToArray();
            return await tags.AllExistAsync(ids, ct);
        }
    }

    public class UpdateValidMediasFavoriteValidator(IEntityExistenceChecker<FileMetaEntity> files)
        : AbstractValidator<UpdateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "一个或多个关联文件不存在";
        protected override async Task<bool> IsValidAsync(UpdateFavoriteCommand command, CancellationToken ct)
        {
            var ids = (command.Media ?? []).Select(media => media.FileId)
                .Where(id => id != Guid.Empty).Distinct().ToArray();
            return await files.AllExistAsync(ids, ct);
        }
    }

    public class UpdateValidCategoryFavoriteValidator(IEntityExistenceChecker<CategoryEntity> categories)
        : AbstractValidator<UpdateFavoriteCommand>
    {
        protected override string ErrorMessage { init; get; } = "Favorite 必须指定一个有效分类";
        protected override async Task<bool> IsValidAsync(UpdateFavoriteCommand command, CancellationToken ct)
            => command.CID is Guid categoryId && await categories.ExistsAsync(categoryId, ct);
    }
}

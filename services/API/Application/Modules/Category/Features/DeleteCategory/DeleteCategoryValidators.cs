using AqLife.Application.Abstractions.Contracts.Category;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Category;
using AqLife.Domain.Entities;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Category.Validator
{
    public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
    {
        protected override string ErrorMessage { init; get; } = "不允许删除默认分类";

        protected override async Task<bool> IsValidAsync(DeleteCategoryCommand command, CancellationToken ct)
            => command.UID != CategoryEntity.DefaultCategoryId;
    }

    public sealed class DeleteCategoryQuoteValidator(
        IApplicationDbContext dbContext,
        IEnumerable<ICategoryReferenceProvider> referenceProviders)
        : AbstractValidator<DeleteCategoryCommand>
    {
        protected override string ErrorMessage { init; get; } = "分类仍被子分类、文件或收藏使用，不能删除";

        protected override async Task<bool> IsValidAsync(DeleteCategoryCommand command, CancellationToken ct)
        {
            var hasChildren = await dbContext.Categories
                .AnyAsync(category => category.ParentId == command.UID, ct);
            if (hasChildren)
                return false;

            foreach (var provider in referenceProviders)
            {
                if (await provider.IsReferencedAsync(command.UID, ct))
                    return false;
            }

            return true;
        }
    }
}
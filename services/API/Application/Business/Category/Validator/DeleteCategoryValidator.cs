using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Category.Validator
{
    public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "不允许删除默认分类";

        private protected override async Task<bool> IsValidAsync(DeleteCategoryCommand command, CancellationToken ct)
        => command.UID != CategoryEntity.DefaultCategoryId;
    }

    public sealed class DeleteCategoryQuoteValidator(IApplicationDbContext dbContext)
        : AbstractValidator<DeleteCategoryCommand>
    {
        private protected override string ErrorMessage { init; get; } = "分类仍被子分类、文件或收藏使用，不能删除";

        private protected override async Task<bool> IsValidAsync(DeleteCategoryCommand command, CancellationToken ct)
        {
            bool hasChildren = await dbContext.Categories.AnyAsync(category => category.ParentId == command.UID, ct);
            if (hasChildren)
                return false;

            var hasFiles = await dbContext.File.AnyAsync(file => file.CategoryUID == command.UID, ct);
            if (hasFiles)
                return false;

            return !await dbContext.FavoriteItems.AnyAsync(favorite => favorite.CategoryUID == command.UID, ct);
        }
    }

}
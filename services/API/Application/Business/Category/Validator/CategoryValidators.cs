using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Validator;

public sealed class CreateCategoryValidator(IApplicationDbContext dbContext)
    : AbstractValidator<CreateCategoryCommand>
{
    private protected override string ErrorMessage { init; get; } = "分类名称无效、已存在或父分类不存在";

    private protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 32)
            return false;

        if (await dbContext.Categories.AnyAsync(category => category.CategoryName == name, ct))
            return false;

        return command.ParentId is null ||
            await dbContext.Categories.AnyAsync(category => category.UID == command.ParentId, ct);
    }
}

public sealed class UpdateCategoryValidator(IApplicationDbContext dbContext)
    : AbstractValidator<UpdateCategoryCommand>
{
    private protected override string ErrorMessage { init; get; } = "分类名称无效、已存在，或父分类不存在/形成循环";

    private protected override async Task<bool> IsValidAsync(UpdateCategoryCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 32 ||
            await dbContext.Categories.AnyAsync(category =>
                category.UID != command.UID && category.CategoryName == name, ct))
            return false;

        if (command.ParentId is not Guid parentId)
            return true;

        if (parentId == command.UID)
            return false;

        var parentMap = await dbContext.Categories
            .AsNoTracking()
            .Select(category => new { category.UID, category.ParentId })
            .ToDictionaryAsync(category => category.UID, category => category.ParentId, ct);

        if (!parentMap.ContainsKey(parentId))
            return false;

        // Walk upward from the proposed parent; reaching this category would create a cycle.
        Guid? current = parentId;
        var visited = new HashSet<Guid>();
        while (current is Guid currentId && visited.Add(currentId))
        {
            if (currentId == command.UID)
                return false;

            current = parentMap.GetValueOrDefault(currentId);
        }

        return true;
    }
}

public sealed class DeleteCategoryValidator(IApplicationDbContext dbContext)
    : AbstractValidator<DeleteCategoryCommand>
{
    private protected override string ErrorMessage { init; get; } = "分类仍被子分类、文件或收藏使用，不能删除";

    private protected override async Task<bool> IsValidAsync(DeleteCategoryCommand command, CancellationToken ct)
    {
        var hasChildren = await dbContext.Categories.AnyAsync(category => category.ParentId == command.UID, ct);
        if (hasChildren)
            return false;

        var hasFiles = await dbContext.File.AnyAsync(file => file.CategoryUID == command.UID, ct);
        if (hasFiles)
            return false;

        return !await dbContext.FavoriteItems.AnyAsync(favorite => favorite.CategoryUID == command.UID, ct);
    }
}

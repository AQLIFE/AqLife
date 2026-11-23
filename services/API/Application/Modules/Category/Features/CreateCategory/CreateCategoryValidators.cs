using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Category;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Category.Validator;

public sealed class CreateCategoryParentValidator(ICategoryPersistence persistence)
    : AbstractValidator<CreateCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "父分类不存在";

    protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    {
        return command.ParentId is null ||
            await persistence.Categories.AnyAsync(category => category.UID == command.ParentId, ct);
    }
}

public sealed class CreateCategoryDefaultValidator(ICategoryPersistence persistence): AbstractValidator<CreateCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "不允许同名的分类";

    protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    => await persistence.Categories.AllAsync(category => category.CategoryName != command.Name, ct);

}

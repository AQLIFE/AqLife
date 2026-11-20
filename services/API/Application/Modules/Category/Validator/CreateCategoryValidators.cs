using AqLife.Application.Abstractions.Persistence;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Category.Validator;

public sealed class CreateCategoryParentValidator(IApplicationDbContext dbContext)
    : AbstractValidator<CreateCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "父分类不存在";

    protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    {
        return command.ParentId is null ||
            await dbContext.Categories.AnyAsync(category => category.UID == command.ParentId, ct);
    }
}

public sealed class CreateCategoryDefaultValidator(IApplicationDbContext dbContext): AbstractValidator<CreateCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "不允许同名的分类";

    protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    => await dbContext.Categories.AllAsync(category => category.CategoryName != command.Name, ct);

}

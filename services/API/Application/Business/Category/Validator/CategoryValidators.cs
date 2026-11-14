using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Validator;

public sealed class CreateCategoryParentValidator(IApplicationDbContext dbContext)
    : AbstractValidator<CreateCategoryCommand>
{
    private protected override string ErrorMessage { init; get; } = "父分类不存在";

    private protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    {
        return command.ParentId is null ||
            await dbContext.Categories.AnyAsync(category => category.UID == command.ParentId, ct);
    }
}

public sealed class CreateCategoryDefaultValidator(IApplicationDbContext dbContext)
    : AbstractValidator<CreateCategoryCommand>
{
    private protected override string ErrorMessage { init; get; } = "不允许同名的分类";

    private protected override async Task<bool> IsValidAsync(CreateCategoryCommand command, CancellationToken ct)
    =>await dbContext.Categories.AnyAsync(category => category.CategoryName != command.Name, ct);
    
}

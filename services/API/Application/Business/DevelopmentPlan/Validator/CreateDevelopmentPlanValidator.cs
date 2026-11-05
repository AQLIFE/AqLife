using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Validator;

public sealed class CreateDevelopmentPlanNameValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateDevelopmentPlanCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划名称不能为空且不能超过 128 个字符";
    private protected override async Task<bool> IsValidAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return !string.IsNullOrWhiteSpace(command.Name) && command.Name.Trim().Length <= 128;
    }

}

public sealed class CreateDevelopmentPlanDescriptionValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateDevelopmentPlanCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划描述不能超过 2000 个字符";
    private protected override async Task<bool> IsValidAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return command.Description is null || command.Description.Length <= 2000;
    }

}

using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
using Plan.DevelopmentPlan;
namespace Plan.DevelopmentPlan.Validator;

public sealed class UpdateDevelopmentPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划不存在";
    protected override async Task<bool> IsValidAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.UID, ct);
    }
    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

//public sealed class UpdateDevelopmentPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateDevelopmentPlanCommand>
//{
//    protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
//    protected override async Task<bool> IsValidAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
//    {
//        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.UID && x.IsArchived, ct);
//    }
//    protected override Exception CreateException(string message) => new DomainLegalityException(message);

//}

public sealed class UpdateDevelopmentPlanNameValidator() : AbstractValidator<UpdateDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划名称不能为空且不能超过 128 个字符";
    protected override async Task<bool> IsValidAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return !string.IsNullOrWhiteSpace(command.Name) && command.Name.Trim().Length <= 128;
    }

}

public sealed class UpdateDevelopmentPlanDescriptionValidator() : AbstractValidator<UpdateDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划描述不能超过 2000 个字符";
    protected override async Task<bool> IsValidAsync(UpdateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return command.Description is null || command.Description.Length <= 2000;
    }

}

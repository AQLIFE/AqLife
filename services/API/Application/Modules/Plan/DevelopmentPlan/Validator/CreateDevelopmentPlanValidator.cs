using Core.Validators;
using Plan.DevelopmentPlan;
namespace Plan.DevelopmentPlan.Validator;

public sealed class CreateDevelopmentPlanNameValidator() : AbstractValidator<CreateDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划名称不能为空且不能超过 128 个字符";
    protected override async Task<bool> IsValidAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return !string.IsNullOrWhiteSpace(command.Name) && command.Name.Trim().Length <= 128;
    }

}

public sealed class CreateDevelopmentPlanDescriptionValidator() : AbstractValidator<CreateDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划描述不能超过 2000 个字符";
    protected override async Task<bool> IsValidAsync(CreateDevelopmentPlanCommand command, CancellationToken ct)
    {
        return command.Description is null || command.Description.Length <= 2000;
    }

}

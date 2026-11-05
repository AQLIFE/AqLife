using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.DevelopmentPlan.Validator;

public sealed class CreateDevelopmentPlanValidator(
    AqLife.Application.Abstractions.Persistence.IApplicationDbContext db)
    : DevelopmentPlanValidatorBase<CreateDevelopmentPlanCommand>(db, "开发计划名称或描述不合法")
{
    private protected override Task<bool> IsValidAsync(CreateDevelopmentPlanCommand source, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(source.Name) || source.Name.Trim().Length > 128)
            return Task.FromResult(Reject(new DomainLegalityException("开发计划名称不能为空且不能超过 128 个字符")));
        if (source.Description?.Length > 2000)
            return Task.FromResult(Reject(new DomainLegalityException("开发计划描述不能超过 2000 个字符")));
        return Task.FromResult(true);
    }
}
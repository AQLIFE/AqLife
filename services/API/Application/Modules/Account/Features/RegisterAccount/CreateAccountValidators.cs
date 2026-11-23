using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Account;
using AqLife.Application.Account.Services;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Validator
{

    public class CreateAccount_InvalidName_ShouldRejectValidator : AbstractValidator<CreateAccountCommand>
    {
        protected override string ErrorMessage { init; get; } = "账户名称不允许存在空格";
        protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !string.IsNullOrWhiteSpace(command.Name) && command.Name.All(c => !char.IsWhiteSpace(c));
    }

    /// <summary>
    /// 创建时账户唯一性检查: 不允许创建第二个账户
    /// </summary>
    /// <param name="dbContext"></param>
    public class SingleAccountSystemValidator(IAccountPersistence persistence) : AbstractValidator<CreateAccountCommand>
    {
        protected override string ErrorMessage { init; get; } = "仅允许注册一个账户";
        protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !await persistence.Accounts.AsNoTracking().AnyAsync(a => a.IsValid);
    }

    /// <summary>
    /// 必须具有系统密钥
    /// </summary>
    /// <param name="options"></param>
    public class CreateAccountServerKeyValidator(SystemInitializationService initializationService) : AbstractValidator<CreateAccountCommand>
    {
        protected override string ErrorMessage { init; get; } = "系统密钥不匹配";
        protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => await initializationService.ValidateSystemKey(command.ServerKey);
    }
}

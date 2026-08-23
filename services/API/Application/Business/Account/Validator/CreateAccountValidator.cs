using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Shared.Options;
using MyLife.Application.Abstractions.Persistence;
using System.Runtime.Intrinsics.Arm;

namespace MyLife.Application.Business.Account.Validator
{
    /// <summary>
    /// 创建时账户名唯一性检查:不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class CreateAccountNameUniquenessValidator(IApplicationDbContext storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "账户名称已存在";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !await storage.Accounts.AsNoTracking().AnyAsync(a => a.Name == command.Name);
    }

    public class CreateAccount_InvalidName_ShouldRejectValidator : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "账户名称不允许存在空格";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !string.IsNullOrWhiteSpace(command.Name) && command.Name.All(c => !char.IsWhiteSpace(c));
    }

    /// <summary>
    /// 创建时账户唯一性检查: 不允许创建第二个账户
    /// </summary>
    /// <param name="storage"></param>
    public class SingleAccountSystemValidator(IApplicationDbContext storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "仅允许注册一个账户";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !await storage.Accounts.AsNoTracking().AnyAsync(a => a.IsValid);
    }

    /// <summary>
    /// 必须具有系统密钥
    /// </summary>
    /// <param name="options"></param>
    public class CreateAccountServerKeyValidator(IOptions<JwtOption> options) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "系统密钥不匹配";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => options.Value.SecretKey == command.ServerKey;        
    }
}

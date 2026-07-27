using Microsoft.EntityFrameworkCore;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Shared.Utils;
using MyLife.Application.Abstractions.Persistence;

namespace MyLife.Application.Business.Account.Validator
{
    /// <summary>
    /// 登录检查
    /// </summary>
    /// <param name="options"></param>
    /// <param name="storage"></param>
    public class AccountCredentialValidator(IApplicationDbContext storage) : AbstractValidator<LoginCommand>
    {
        private protected override string ErrorMessage { init; get; } = "登录失败";
        private protected override async Task<bool> IsValidAsync(LoginCommand command, CancellationToken ct)
        {
            var entity = await storage.Accounts.AsNoTracking().FirstAsync(a => a.IsValid);
            return entity.LoginName == command.AccountName && entity.LoginPasswordHash == FastHash.GetSha256Hash(command.SecretKey);
        }
    }
}

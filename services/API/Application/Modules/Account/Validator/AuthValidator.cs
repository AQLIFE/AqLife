using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Utils;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Account.Validator
{
    /// <summary>
    /// 登录检查
    /// </summary>
    /// <param name="options"></param>
    /// <param name="storage"></param>
    public class AccountCredentialValidator(IApplicationDbContext storage) : AbstractValidator<LoginCommand>
    {
        protected override string ErrorMessage { init; get; } = "登录失败";
        protected override async Task<bool> IsValidAsync(LoginCommand command, CancellationToken ct)
        {
            var entity = await storage.Accounts.AsNoTracking().FirstAsync(a => a.IsValid);
            return entity.LoginName == command.AccountName && entity.LoginPasswordHash == FastHash.GetSha256Hash(command.SecretKey);
        }
    }
}

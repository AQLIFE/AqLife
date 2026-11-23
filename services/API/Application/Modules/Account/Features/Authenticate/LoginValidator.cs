using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Account;
using AqLife.Shared.Utils;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Validator
{
    /// <summary>
    /// 登录检查
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dbContext"></param>
    public class AccountCredentialValidator(IApplicationDbContext dbContext) : AbstractValidator<LoginCommand>
    {
        protected override string ErrorMessage { init; get; } = "登录失败";
        protected override async Task<bool> IsValidAsync(LoginCommand command, CancellationToken ct)
        {
            var entity = await dbContext.Accounts.AsNoTracking().FirstAsync(a => a.IsValid);
            return entity.LoginName == command.AccountName && entity.LoginPasswordHash == FastHash.GetSha256Hash(command.SecretKey);
        }
    }
}

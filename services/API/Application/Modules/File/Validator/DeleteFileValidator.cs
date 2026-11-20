using AqLife.Application.Abstractions.Persistence;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.File.Validator
{

    /// <summary>
    /// 不允许删除和账户存在关联的文件
    /// </summary>
    /// <param name="accountSearch"></param>
    public class FileDeleteValidtor(IApplicationDbContext dbContext) : AbstractValidator<DeleteFileCommand>
    {
        protected override string ErrorMessage { init; get; } = "预期删除的文件被其他业务依赖着,不允许删除";
        protected override async Task<bool> IsValidAsync(DeleteFileCommand command, CancellationToken ct)
        {
            bool useByAvatar = await dbContext.Accounts.AnyAsync(e => e.IsValid && e.Avatar == command.UID, ct);

            bool usedBySubscription = await dbContext.Accounts
                .Where(e => e.IsValid)
                .SelectMany(e => e.Subscriptions)
                .AnyAsync(e => e.SubscriptionIcon == command.UID, ct);

            bool usedByFavorite = await dbContext.FavoriteItems
                .SelectMany(e => e.Media)
                .AnyAsync(e => e.FileId == command.UID, ct);
            return !(useByAvatar || usedBySubscription || usedByFavorite);// 反之不包含即可允许删除
        }
    }
}

using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.Account.Search;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Validator
{

    /// <summary>
    /// 不允许删除和账户存在关联的文件
    /// </summary>
    /// <param name="accountSearch"></param>
    public class FileDeleteValidtor(IApplicationDbContext dbContext) : AbstractValidator<DeleteFileCommand>
    {
        private protected override string ErrorMessage { init; get; } = "预期删除的文件被其他业务依赖着,不允许删除";
        private protected override async Task<bool> IsValidAsync(DeleteFileCommand command, CancellationToken ct)
        {
            var entity = dbContext.Accounts.Where(e => e.IsValid);
                HashSet<Guid> dependencyIds = new();
            if (await entity.AnyAsync(ct) && await entity.SingleAsync(ct) is AccountEntity account)
            {
                if (account.Avatar != Guid.Empty) dependencyIds.Add(account.Avatar);
                IQueryable<Guid> subs = account.Subscriptions.Where(e => e.SubscriptionIcon != Guid.Empty).Select(e => e.SubscriptionIcon).AsQueryable();
                subs.Select(dependencyIds.Add);// 添加进集合,所有订阅依赖的文件ID
            }

            var favoriteies = dbContext.FavoriteItems.Include(e => e.Media).SelectMany(e => e.Media.Select(e => e.FileId));// 给出 所有收藏依赖的文件ID
            if (await favoriteies.AnyAsync(ct))
                favoriteies.Select(dependencyIds.Add);
            return !dependencyIds.Contains(command.UID);// 反之不包含即可允许删除
        }
    }
}

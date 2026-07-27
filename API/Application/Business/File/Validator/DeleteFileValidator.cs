using MyLife.Application.Business.Account.Search;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;

namespace MyLife.Application.Business.File.Validator
{

    /// <summary>
    /// 不允许删除和账户存在关联的文件
    /// </summary>
    /// <param name="accountSearch"></param>
    public class FileDeleteValidtor(AccountSearch accountSearch) : AbstractValidator<DeleteFileCommand>
    {
        private protected override string ErrorMessage { init; get; } = "预期删除的文件被其他业务依赖着,不允许删除";
        private protected override async Task<bool> IsValidAsync(DeleteFileCommand command, CancellationToken ct)
        {
            var entity = await accountSearch.SearchAsync(new AccountQuery(), ct);
            if (entity.Count() > 0 && entity.First() is AccountEntity account)
            {
                List<Guid?> accountFIles = new List<Guid?>();
                IEnumerable<Guid?> subs = account.Subscriptions.Where(e => e.SubscriptionIcon != null).Select(e => e.SubscriptionIcon).AsEnumerable();
                accountFIles.AddRange(subs);
                accountFIles.Add(account.Avatar);
                return accountFIles.Contains(command.UID);
            }
            return true;//如果搜不到账户信息,则默认该ID 与 Account 没有业务关系

        }
    }
}

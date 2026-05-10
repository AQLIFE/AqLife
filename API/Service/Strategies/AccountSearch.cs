using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;

namespace MyLife.Service.Strategies
{
    public class AccountNameStrategy(AppStorage storage) : IAccountStrategy
    {
        public (bool IsValid, string Message) Check(AccountDto account)
        => storage.Account.AsNoTracking().Any(a => a.Name == account.Name)
            ? (false, "账户名称已存在")
            : (true, string.Empty);
    }
}

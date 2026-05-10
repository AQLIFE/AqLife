using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;

namespace MyLife.Service.Implementations
{
    public class AccountService(
        AppStorage storage,
        IEnumerable<IAccountStrategy> _strategies,
        IGenericsMapper<AccountEntity, AccountDto> accountMapper,
        IGenericsMapper<SubscriptionEntity, SubscriptionDto> subscriptionMapper
        )
    {
        private (bool IsValid, string Message) CompoundCheck(AccountDto account)
        {
            return _strategies.Select(strategy => strategy.Check(account)).FirstOrDefault(result => !result.IsValid) is (false, string message)
                ? (false, message)
                : (true, string.Empty);
        }

        public async Task<string> AddAccountAsync(AccountDto account)
        {
            var (isValid, message) = CompoundCheck(account);
            if (isValid)
            {
                var existingAccount = await storage.Account.Where(e => e.IsValid == true).FirstOrDefaultAsync();
                if (existingAccount != null)
                {
                    existingAccount.IsValid = false;
                }

                AccountEntity user = accountMapper.Assembly(account);
                user.IsValid = true;

                storage.Account.Add(user);
                await storage.SaveChangesAsync();

                return user.Name;
            }
            else
            {
                return message;
            }
        }

        public async Task<int> AddSubscriptionAsync(params SubscriptionDto[] subscriptionDtos)
        {
            if( subscriptionDtos == null || subscriptionDtos.Length == 0)
                return 0;
            var list = subscriptionDtos.Select(e => subscriptionMapper.Assembly(e)).ToList();
            var existingAccount = await storage.Account.Where(e => e.IsValid == true).FirstOrDefaultAsync();
            if (existingAccount != null)
                list.ForEach(e => { e.UID = existingAccount.UID; e.Account = existingAccount; });
            storage.Subscription.AddRange(list);
            return await storage.SaveChangesAsync();
        }
    }
}

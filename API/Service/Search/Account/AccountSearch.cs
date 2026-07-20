using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Repository;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;
namespace MyLife.Service.Search.Account
{
    public class AccountSearch(AppStorage storage, IEnumerable<ISearchStrategy<AccountEntity>> searchStrategies)
    {
        //private bool IsValid { init; get; } = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        public async Task<IEnumerable<AccountEntity>> SearchAsync(CancellationToken ct, Guid? UID = null, bool isPrivate = false)
        {
            var strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(UID));
            if (strategy == null) return [];
            var query = storage.Account.Include(e=>e.Subscriptions).AsQueryable();
            return await strategy.ExecuteAsync(query,UID);
        }

    }
}

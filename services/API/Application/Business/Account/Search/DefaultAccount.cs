using Microsoft.EntityFrameworkCore;
using MyLife.Application.Search;
using MyLife.Domain.Entities;

namespace MyLife.Application.Business.Account.Search
{
    public class DefaultAccount : AllSearchStrategyBase<AccountEntity, EntitySearchCriteria>
    {
        public override async Task<IEnumerable<AccountEntity>> ExecuteAsync(
            IQueryable<AccountEntity> queryable,
            EntitySearchCriteria c,
            CancellationToken ct = default)
        => await queryable.Where(e => e.IsValid).ToListAsync(ct);
        //public async Task<IEnumerable<AccountEntity>> ExecuteAsync(IQueryable<AccountEntity> queryable, Guid? UID = null)
        //=>await queryable.Where(e => e.IsValid).ToListAsync();
        //public bool IsMatch(Guid? UID = null) => UID == null;

    }
}

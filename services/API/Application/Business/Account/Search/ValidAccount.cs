using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Account.Search
{
    public class ValidAccount : FilteredSearchStrategyBase<AccountEntity, EntitySearchCriteria>
    {
        protected override IQueryable<AccountEntity> ApplyKeywordFilter(IQueryable<AccountEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
        //public bool IsMatch(Guid? UID) => UID is Guid;
        //public async Task<IEnumerable<AccountEntity>> ExecuteAsync(
        //    IQueryable<AccountEntity> queryable,
        //    Guid? UID = null)
        //=> await queryable.Where(e => e.UID == UID).ToListAsync();
    }
}

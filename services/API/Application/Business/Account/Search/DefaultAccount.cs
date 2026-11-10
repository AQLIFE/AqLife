using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Account.Search
{
    public class DefaultAccount(IHttpContextAccessor httpContext) : AllSearchStrategyBase<AccountEntity, EntitySearchCriteria>(httpContext)
    {
        public override IQueryable<AccountEntity> Execute(
            IQueryable<AccountEntity> queryable,
            EntitySearchCriteria c,
            CancellationToken ct = default)
        => queryable.Where(e => e.IsValid);
    }
}

using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Account.Search
{
    public sealed class DefaultAccount(IHttpContextAccessor httpContext) : AllSearchStrategyBase<AccountEntity, EntitySearchCriteria>(httpContext)
    {
    }
}

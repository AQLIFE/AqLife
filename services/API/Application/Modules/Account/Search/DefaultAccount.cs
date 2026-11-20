using AqLife.Domain.Entities;
using Core.Search;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Modules.Account.Search
{
    public sealed class DefaultAccount(IHttpContextAccessor httpContext) : AllSearchStrategyBase<AccountEntity, EntitySearchCriteria>(httpContext)
    {
    }
}

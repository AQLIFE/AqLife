using AqLife.Application.Search;
using AqLife.Domain.Entities.Favorite;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Favorite.Search
{
    public sealed class AllFavoriteSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<FavoriteItemEntity, FavoriteSearchCriteria>(httpContext)
    {
        public override bool IsMatch(FavoriteSearchCriteria c)
        => base.IsMatch(c) && c.TagId is null && c.CategoryId is null;
    }
}

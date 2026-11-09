using AqLife.Application.Search;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Favorite.Search
{
    public class FilteredFavoriteSearchStrategy(IHttpContextAccessor httpContext) : FilteredSearchStrategyBase<FavoriteItemEntity, FavoriteSearchCriteria>
    {
        private bool IsAuth = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        public override bool IsMatch(FavoriteSearchCriteria c)
        => c.UID is not null || !string.IsNullOrWhiteSpace(c.Keyword) || c.CategoryId is not null || c.TagId is not null;

        protected override IQueryable<FavoriteItemEntity> ApplyKeywordFilter(IQueryable<FavoriteItemEntity> q, string keyword)
        {
            return q.Where(e => e.Title.Contains(keyword));
        }

        public override IQueryable<FavoriteItemEntity> Execute(IQueryable<FavoriteItemEntity> queryable, FavoriteSearchCriteria criteria, CancellationToken ct = default)
        {
            return criteria switch
            {
                { UID: { } uid } => queryable.Where(x => x.UID == uid),

                { Keyword: { Length: > 0 } keyword } => queryable.Where(x => x.Title.Contains(keyword) || x.Description.Contains(keyword)),

                { CategoryId: { } categoryId } => queryable.Where(x => x.FavoriteCategoryId == categoryId),

                { TagId: { } tagId } => queryable.Where(x => x.Tags.Any(t => t.TagId == tagId)),

                _ => throw new RequestFailException("不合规的操作，该请求不应被处理")
            };
        }
    }
}

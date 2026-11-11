using AqLife.Application.Search;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.Favorite.Search
{
    public sealed class FilteredFavoriteSearchStrategy
        : FilteredSearchStrategyBase<FavoriteItemEntity, FavoriteSearchCriteria>
    {
        public override bool IsMatch(FavoriteSearchCriteria criteria)
            => base.IsMatch(criteria) || criteria.CategoryId is Guid || criteria.TagId is Guid;

        protected private override IQueryable<FavoriteItemEntity> ApplyKeywordFilter(
            IQueryable<FavoriteItemEntity> queryable, string keyword)
            => queryable.Where(e => e.Title.Contains(keyword));

        public override IQueryable<FavoriteItemEntity> Execute(
            IQueryable<FavoriteItemEntity> queryable,
            FavoriteSearchCriteria criteria,
            CancellationToken ct = default)
        => criteria switch
        {
            { UID: { } uid } => queryable.Where(x => x.UID == uid),
            { Keyword: { } keyword } when !string.IsNullOrWhiteSpace(keyword) =>
                queryable.Where(x => x.Title.Contains(keyword) || x.Description.Contains(keyword)),
            { CategoryId: { } categoryId } =>
                queryable.Where(x => x.CategoryUID == categoryId),
            { TagId: { } tagId } =>
                queryable.Where(x => x.Tags.Any(t => t.TagId == tagId)),
            _ => throw new RequestFailException("不合规的操作，该请求不应被处理")
        };
    }
}

using AqLife.Application.Search;
using AqLife.Domain.Entities.Favorite;

namespace AqLife.Application.Business.Favorite.Search
{
    public class FilteredFavoriteSearchStrategy
        : FilteredSearchStrategyBase<FavoriteItemEntity, FavoriteSearchCriteria>
    {
        // This strategy also handles an empty criteria set; the switch default is the unfiltered query.
        public override bool IsMatch(FavoriteSearchCriteria criteria) => true;

        protected override IQueryable<FavoriteItemEntity> ApplyKeywordFilter(
            IQueryable<FavoriteItemEntity> queryable, string keyword)
            => queryable.Where(e => e.Title.Contains(keyword));

        public override IQueryable<FavoriteItemEntity> Execute(
            IQueryable<FavoriteItemEntity> queryable,
            FavoriteSearchCriteria criteria,
            CancellationToken ct = default)
        {
            return criteria switch
            {
                { UID: { } uid } => queryable.Where(x => x.UID == uid),
                { Keyword: { } keyword } when !string.IsNullOrWhiteSpace(keyword) =>
                    queryable.Where(x => x.Title.Contains(keyword) || x.Description.Contains(keyword)),
                { CategoryId: { } categoryId } =>
                    queryable.Where(x => x.CategoryUID == categoryId),
                { TagId: { } tagId } =>
                    queryable.Where(x => x.Tags.Any(t => t.TagId == tagId)),
                _ => queryable
            };
        }
    }
}

using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.Favorite;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Favorite.Search
{
    public sealed class FavoriteSearch(IApplicationDbContext dbContext,
        ISearchMapper<FavoriteQuery, PageResult<FavoriteDto>, FavoriteSearchCriteria> queryMapper,
        IHttpContextAccessor httpContext,
        IEnumerable<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>> searchStrategies)
        : BaseSearch<FavoriteQuery, PageResult<FavoriteDto>, FavoriteItemEntity, FavoriteSearchCriteria>(dbContext, httpContext, searchStrategies)
    {
        protected override FavoriteSearchCriteria MapToCriteria(FavoriteQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<FavoriteItemEntity> ApplyDefaultOrder(IQueryable<FavoriteItemEntity> queryble, FavoriteQuery query)
            => queryble.OrderBy(x => x.SortOrder).ThenByDescending(x => x.CreatedAt);

        protected override IQueryable<FavoriteItemEntity> BuildBaseQuery(IQueryable<FavoriteItemEntity> queryable, FavoriteQuery query)
        => queryable.Include(e => e.Category).Include(x => x.Tags).Include(f => f.Media);
        protected override IQueryable<FavoriteItemEntity> ApplyResultPolicy(IQueryable<FavoriteItemEntity> queryable, FavoriteQuery query)
        => IsValid ? queryable : queryable.Where(e => e.IsPublished);// 登录后可以查看所有收藏夹，未登录用户只能查看已发布的收藏夹
    }
}

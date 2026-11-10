using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite.Search
{
    public readonly record struct FavoriteSearchCriteria(Guid? UID = null, string? Keyword = null, Guid? CategoryId = null, Guid? TagId = null) : ISearchCriteria;
    public sealed class FavoriteSearch(IApplicationDbContext dbContext, QueryMapper queryMapper, IHttpContextAccessor httpContext, IEnumerable<ISearchStrategy<FavoriteItemEntity, FavoriteSearchCriteria>> searchStrategies)
        : BaseSearch<FavoriteQuery, FavoriteItemEntity, FavoriteDto, FavoriteSearchCriteria>(dbContext, httpContext, searchStrategies)
    {
        protected private override FavoriteSearchCriteria MapToCriteria(FavoriteQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<FavoriteItemEntity> ApplyDefaultOrder(IQueryable<FavoriteItemEntity> queryble, FavoriteQuery query)
            => queryble.OrderBy(x => x.SortOrder).ThenByDescending(x => x.CreatedAt);

        protected private override IQueryable<FavoriteItemEntity> BuildBaseQuery(IQueryable<FavoriteItemEntity> queryable, FavoriteQuery query)
        => queryable.Include(e => e.Category).Include(x => x.Tags).Include(f => f.Media);
        private protected override IQueryable<FavoriteItemEntity> ApplyResultPolicy(IQueryable<FavoriteItemEntity> queryable, FavoriteQuery query)
        => IsValid ? queryable : queryable.Where(e => e.IsPublished);// 登录后可以查看所有收藏夹，未登录用户只能查看已发布的收藏夹
    }
}

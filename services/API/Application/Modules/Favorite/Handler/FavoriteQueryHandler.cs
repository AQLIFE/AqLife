using AqLife.Application.Abstractions.Search;
using AqLife.Application.Favorite;
using AqLife.Domain.Entities.Favorite;
using Core.Mappers;
using MediatR;

namespace AqLife.Application.Favorite.Handler
{
    public class FavoriteQueryHandler(ISearch<FavoriteQuery, PageResult<FavoriteDto>, FavoriteItemEntity> favoriteSearch, PageResultMapper<FavoriteItemEntity, FavoriteDto> mapper) : IRequestHandler<FavoriteQuery, PageResult<FavoriteDto>>
    {
        public async Task<PageResult<FavoriteDto>> Handle(FavoriteQuery query, CancellationToken ct)
        {
            PageResult<FavoriteItemEntity> result = await favoriteSearch.SearchPageAsync(query, ct);
            return mapper.ToDto(result);
        }
    }
}

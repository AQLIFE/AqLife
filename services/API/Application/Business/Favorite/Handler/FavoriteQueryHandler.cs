using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.Favorite.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.IView;
using MediatR;

namespace AqLife.Application.Business.Favorite.Handler
{
    public class FavoriteQueryHandler(ISearch<FavoriteQuery, FavoriteItemEntity> favoriteSearch, PageResultMapper<FavoriteItemEntity, FavoriteDto> mapper) : IRequestHandler<FavoriteQuery, PageResult<FavoriteDto>>
    {
        public async Task<PageResult<FavoriteDto>> Handle(FavoriteQuery query, CancellationToken ct)
        {
            PageResult<FavoriteItemEntity> result = await favoriteSearch.SearchPageAsync(query, ct);
            return mapper.ToDto(result);
        }
    }
}

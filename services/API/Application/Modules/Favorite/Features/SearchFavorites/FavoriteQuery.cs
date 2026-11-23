using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;
namespace AqLife.Application.Favorite;
public record FavoriteQuery(Guid? UID = null, Guid? CategoryId = null, Guid? TagId = null, string? CName = null, int Page = 1, int PageSize = 10)
    : IPageQuery, IQuery<PageResult<FavoriteDto>>;

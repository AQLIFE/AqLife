using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;

namespace AqLife.Application.Tag;

public record TagQuery(Guid? UID = null, string? Tag = null, int Page = 1, int PageSize = 50)
    : IPageQuery, IQuery<PageResult<TagDto>>;

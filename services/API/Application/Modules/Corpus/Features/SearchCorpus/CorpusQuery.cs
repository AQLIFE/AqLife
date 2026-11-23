using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Search;

namespace AqLife.Application.Corpus;

public record CorpusQuery(Guid? UID = null, string? Content = null, int Page = 1, int PageSize = 3)
    : IPageQuery, IQuery<PageResult<CorpusDto>>;

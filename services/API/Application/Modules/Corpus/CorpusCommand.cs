using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;

namespace Corpus
{
    public record RandomCorpusQuery : IQuery<string>;
    public record CorpusQuery(Guid? UID = null, string? Content = null, int Page = 1, int PageSize = 3) : IPageQuery, IQuery<PageResult<CorpusDto>>;
    public record CreateCorpusCommand(string Content) : ICreateCommand;
    public record UpdateCorpusCommand(Guid UID, string Content) : IUpdateCommand, IRequireValidEntity<CorpusEntity>;
    public record DeleteCorepusCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<CorpusEntity>;
}

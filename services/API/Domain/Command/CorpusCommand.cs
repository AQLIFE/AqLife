using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;

namespace AqLife.Domain.Command
{
    public record RandomCorpusQuery : IQuery<string>;
    public record CorpusQuery(Guid? UID = null, string? Content = null) : IQuery<IEnumerable<CorpusDto>>;
    public record CreateCorpusCommand(string Content) : ICreateCommand;
    public record UpdateCorpusCommand(Guid UID, string Content) : IUpdateCommand, IRequireValidEntity<CorpusEntity>;
    public record DeleteCorepusCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<CorpusEntity>;
}

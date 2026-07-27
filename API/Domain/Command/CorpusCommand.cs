using MyLife.Domain.CommandInterface;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;

namespace MyLife.Domain.Command
{
    public record RandomCorpusQuery : IQuery<CorpusDto>;
    public record CorpusQuery(Guid? UID = null, string? Content = null) : IQuery<IEnumerable<CorpusDto>>;
    public record CreateCorpusCommand(string Content) : ICreateCommand;
    public record UpdateCorpusCommand(Guid UID, string Content) : IUpdateCommand, IRequireValidEntity<CorpusEntity>;
    public record DeleteCorepusCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<CorpusEntity>;
}

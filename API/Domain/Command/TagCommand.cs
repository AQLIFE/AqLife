using MyLife.Domain.CommandInterface;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace MyLife.Domain.Command
{
    public record TagQuery(Guid? UID = null, string? Tag = null) : IQuery<IEnumerable<TagDto>?>;
    public record CreateTagCommand([Required(ErrorMessage = "TagName is required")]string Name, string? AliasName, bool IsCategory) : ICreateCommand;
    public record UpdateTagCommand(Guid UID, string TagName, string? AliasName, bool IsCategory) : IUpdateCommand, IRequireValidEntity<TagEntity>;
    public record DeleteTagCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<TagEntity>;
}

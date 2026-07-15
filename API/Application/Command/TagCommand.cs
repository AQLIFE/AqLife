using MyLife.Data.Entities;
using MyLife.Shared.Command;
using MyLife.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MyLife.Application.Command
{
    public record TagQuery(Guid? UID = null, string? Tag = null) : IQuery<IEnumerable<TagDto>?>;
    public record CreateTagCommand([Required(ErrorMessage = "必须具有一个标签名称")]string TagName, string? AliasName, bool IsCategory) : ICreateCommand;
    public record UpdateTagCommand(Guid UID, string TagName, string? AliasName, bool IsCategory) : IUpdateCommand, IRequireValidEntity<TagEntity>;
    public record DeleteTagCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<TagEntity>;
}

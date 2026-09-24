using AqLife.Domain.CommandInterface;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command
{
    public record CategoryQuery(bool IsAll=false) : IQuery<IEnumerable<BlogCategoryStatistics>?>;
    public record TagQuery(Guid? UID = null, string? Tag = null,int Page=1,int PageSize=50) :IPageQuery ,IQuery<PageResult<TagDto>?>;
    public record CreateTagCommand([Required(ErrorMessage = "TagName is required")] string Name, string? AliasName, bool IsCategory) : ICreateCommand;
    public record UpdateTagCommand(Guid UID, string TagName, string? AliasName, bool IsCategory) : IUpdateCommand, IRequireValidEntity<TagEntity>;
    public record DeleteTagCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<TagEntity>;
}

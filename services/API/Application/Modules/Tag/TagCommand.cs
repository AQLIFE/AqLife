using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;
using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.IView;
using AqLife.Application.Abstractions.Search;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Application.Modules.Tag
{
    public record TagQuery(Guid? UID = null, string? Tag = null, int Page = 1, int PageSize = 50)
        : IPageQuery, IQuery<PageResult<TagDto>>;

    public record CreateTagCommand(
        [Required(ErrorMessage = "TagName is required")] string Name,
        string? AliasName) : ICreateCommand;

    public record UpdateTagCommand(Guid UID, string TagName, string? AliasName)
        : IUpdateCommand, IRequireValidEntity<TagEntity>;

    public record DeleteTagCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<TagEntity>;
}

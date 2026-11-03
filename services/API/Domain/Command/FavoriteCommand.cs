using AqLife.Domain.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command;

public record FavoriteQuery(Guid? UID = null, string? ContentType = null, Guid? TagId = null,
    int Page = 1, int PageSize = 12, bool IncludeUnpublished = false)
    : IPageQuery, IQuery<PageResult<FavoriteDto>>;

public record CreateFavoriteCommand(
    [Required, StringLength(160)] string Title,
    string? Description,
    [Required, StringLength(32)] string ContentType,
    string? SourceUrl,
    int SortOrder = 0,
    IEnumerable<Guid>? FileIds = null,
    IEnumerable<Guid>? TagIds = null) : ICreateCommand<Guid>;

public record UpdateFavoriteCommand(
    Guid UID,
    [Required, StringLength(160)] string Title,
    string? Description,
    [Required, StringLength(32)] string ContentType,
    string? SourceUrl,
    int SortOrder = 0,
    IEnumerable<Guid>? FileIds = null,
    IEnumerable<Guid>? TagIds = null) : IUpdateCommand;

public record SetFavoritePublishedCommand(Guid UID, bool IsPublished) : IUpdateCommand;
public record DeleteFavoriteCommand(Guid UID) : IDeleteCommand;

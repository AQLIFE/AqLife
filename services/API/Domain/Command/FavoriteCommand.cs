using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;
using System.ComponentModel.DataAnnotations;

namespace AqLife.Domain.Command;

public record FavoriteQuery(Guid? UID = null, Guid? CategoryId = null, Guid? TagId = null, string? CName = null,
    int Page = 1, int PageSize = 10)
    : IPageQuery, IQuery<PageResult<FavoriteDto>>;

public record FavoriteMediaInput(Guid FavoriteId,Guid FileId,int SortOrder = 0) : IUpdateCommand;

public record CreateFavoriteCommand(
    [Required, StringLength(160)] string Title,
    string Description,
    Guid CID,
    string? SourceUrl,
    int SortOrder = 0,
    IEnumerable<Guid>? Media = null,
    IEnumerable<Guid>? TagIds = null) : ICreateCommand<Guid>;

public record UpdateFavoriteCommand(
    Guid UID,
    [Required, StringLength(160)] string Title,
    Guid? CID,
    string? Description,
    string? SourceUrl,
    int SortOrder = 0,
    IEnumerable<FavoriteMediaInput>? Media = null,
    IEnumerable<Guid>? TagIds = null
    ) : IUpdateCommand;

public record SetFavoritePublishedCommand(Guid UID, bool IsPublished) : IUpdateCommand;
public record DeleteFavoriteCommand(Guid UID) : IDeleteCommand;

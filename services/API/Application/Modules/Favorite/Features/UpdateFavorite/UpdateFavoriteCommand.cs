using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Favorite;
public record FavoriteMediaInput(Guid FavoriteId, Guid FileId, int SortOrder = 0) : IUpdateCommand;
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

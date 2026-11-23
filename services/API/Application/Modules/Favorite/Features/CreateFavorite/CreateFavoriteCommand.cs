using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Favorite;
public record CreateFavoriteCommand(
    [Required, StringLength(160)] string Title,
    string Description,
    Guid CID,
    string? SourceUrl,
    int SortOrder = 0,
    IEnumerable<Guid>? Media = null,
    IEnumerable<Guid>? TagIds = null) : ICreateCommand<Guid>;

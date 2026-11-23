using AqLife.Application.Abstractions.Commands;
namespace AqLife.Application.Favorite;
public record DeleteFavoriteCommand(Guid UID) : IDeleteCommand;

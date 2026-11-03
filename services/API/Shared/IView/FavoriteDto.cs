namespace AqLife.Shared.IView;

public record FavoriteMediaDto(Guid FileId, string Role, int SortOrder);
public record FavoriteDto(
    Guid Uid,
    string Title,
    string Description,
    string ContentType,
    string? SourceUrl,
    int SortOrder,
    bool IsPublished,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<FavoriteMediaDto> Media,
    IReadOnlyList<Guid> TagIds);

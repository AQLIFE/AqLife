namespace AqLife.Shared.IView;

public record FavoriteMediaDto(Guid FavoriteId, Guid FileId, string Role, int SortOrder) : IEntityDto;
public record FavoriteDto(
    Guid Uid,
    string Title,
    string Description,
    string? SourceUrl,
    int SortOrder,
    bool IsPublished,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    Guid? Cid,
    IEnumerable<FavoriteMediaDto> Media,
    IEnumerable<Guid> TagIds) : IEntityDto;

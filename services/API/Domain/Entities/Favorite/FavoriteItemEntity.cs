using AqLife.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.Favorite;

[Table("FavoriteItems")]
public class FavoriteItemEntity : IEntity
{
    [Key]
    public Guid UID { get; init; } = Guid.NewGuid();

    [Required, StringLength(160)]
    public string Title { get; private set; } = string.Empty;

    [Column(TypeName = "text")]
    public string Description { get; private set; } = string.Empty;

    [Required, StringLength(32)]
    public string ContentType { get; private set; } = "Other";

    [StringLength(2048)]
    public string? SourceUrl { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsPublished { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public ICollection<FavoriteMediaEntity> Media { get; set; } = [];

    public ICollection<FavoriteTagEntity> Tags { get; set; } = [];

    private FavoriteItemEntity() { }

    public FavoriteItemEntity(string title, string description, string contentType, string? sourceUrl, int sortOrder)
    {
        Update(title, description, contentType, sourceUrl, sortOrder);
    }

    public void Update(string title, string description, string contentType, string? sourceUrl, int sortOrder)
    {
        Title = title.Trim();
        Description = description ?? string.Empty;
        ContentType = contentType.Trim();
        SourceUrl = string.IsNullOrWhiteSpace(sourceUrl) ? null : sourceUrl.Trim();
        SortOrder = sortOrder;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetPublished(bool published)
    {
        IsPublished = published;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

[Table("FavoriteMedia")]
public class FavoriteMediaEntity
{
    public Guid FavoriteId { get; private set; }
    public Guid FileId { get; private set; }
    [Required, StringLength(24)]
    public string Role { get; private set; } = "Image";
    public int SortOrder { get; private set; }

    public FavoriteItemEntity? Favorite { get; private set; }

    private FavoriteMediaEntity() { }

    public FavoriteMediaEntity(Guid favoriteId, Guid fileId, string role, int sortOrder)
    {
        FavoriteId = favoriteId;
        FileId = fileId;
        Role = role;
        SortOrder = sortOrder;
    }
}

[Table("FavoriteTags")]
public class FavoriteTagEntity
{
    public Guid FavoriteId { get; private set; }
    public Guid TagId { get; private set; }
    public FavoriteItemEntity? Favorite { get; private set; }

    private FavoriteTagEntity() { }

    public FavoriteTagEntity(Guid favoriteId, Guid tagId)
    {
        FavoriteId = favoriteId;
        TagId = tagId;
    }
}

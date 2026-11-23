using AqLife.Domain.Entities;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Tag 模块专属持久化入口。
/// </summary>
public interface ITagPersistence
{
    IQueryable<TagEntity> Tags { get; }
    Task<TagEntity?> FindAsync(Guid uid, CancellationToken cancellationToken = default);
    Task AddTagAsync(TagEntity tag, CancellationToken cancellationToken = default);
    void RemoveTag(TagEntity tag);
}

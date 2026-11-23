using AqLife.Domain.Entities.File;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// File 模块专属持久化入口。
/// </summary>
public interface IFilePersistence
{
    IQueryable<FileMetaEntity> Files { get; }
    Task<FileMetaEntity?> FindAsync(Guid uid, CancellationToken cancellationToken = default);
    Task AddFileAsync(FileMetaEntity file, CancellationToken cancellationToken = default);
    void RemoveFile(FileMetaEntity file);
    Task<Dictionary<string, Guid>> FindExistingFilesAsync(
        IEnumerable<string> hashes,
        bool isTemplate,
        CancellationToken cancellationToken = default);
}

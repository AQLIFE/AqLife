using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Domain.Entities;

namespace AqLife.Application.File.Service;

/// <summary>
/// File 模块对外提供的文件资产业务服务。
/// 跨模块删除会先询问各引用模块；仍被引用的文件不会被删除。
/// </summary>
public sealed class FileAssetService(
    FileWriter fileWriter,
    FileDeleter fileDeleter,
    IFilePublishService filePublishService,
    IEnumerable<IFileReferenceProvider> referenceProviders) : IFileAssetService
{
    public Task<Guid> UploadAsync(
        Microsoft.AspNetCore.Http.IFormFile file,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        return fileWriter.WriteAsync(
            new CreateFileCommand(file, categoryId),
            cancellationToken);
    }

    public Task<Guid> UploadToDefaultCategoryAsync(
        Microsoft.AspNetCore.Http.IFormFile file,
        CancellationToken cancellationToken = default)
        => UploadAsync(file, CategoryEntity.DefaultCategoryId, cancellationToken);

    public Task<Guid> PublishAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
        => filePublishService.PublishAsync(fileId, cancellationToken);

    public Task<int> PublishAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default)
        => filePublishService.PublishAsync(fileIds, cancellationToken);

    public async Task DeleteAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileIds);

        var deletableFileIds = new List<Guid>();
        foreach (var fileId in fileIds.Where(id => id != Guid.Empty).Distinct())
        {
            var isReferenced = false;
            foreach (var provider in referenceProviders)
            {
                if (await provider.IsReferencedAsync(fileId, cancellationToken))
                {
                    isReferenced = true;
                    break;
                }
            }

            if (!isReferenced)
                deletableFileIds.Add(fileId);
        }

        if (deletableFileIds.Count > 0)
            await fileDeleter.DeleteAsync(deletableFileIds, cancellationToken);
    }
}

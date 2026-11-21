using AqLife.Application.Abstractions.Contracts.File;

namespace AqLife.Application.Modules.File.Service;

/// <summary>
/// File 模块对外提供的文件资产应用服务。
/// 聚合上传、发布和删除用例，避免其他模块依赖 File 模块内部服务。
/// </summary>
public sealed class FileAssetService(
    FileWriter fileWriter,
    FileDeleter fileDeleter,
    IFilePublishService filePublishService) : IFileAssetService
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

    public Task<Guid> PublishAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
        => filePublishService.PublishAsync(fileId, cancellationToken);

    public Task<int> PublishAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default)
        => filePublishService.PublishAsync(fileIds, cancellationToken);

    public Task DeleteAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default)
        => fileDeleter.DeleteAsync(fileIds, cancellationToken);
}

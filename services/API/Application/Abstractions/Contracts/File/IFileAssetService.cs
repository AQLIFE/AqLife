using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Abstractions.Contracts.File;

/// <summary>
/// 面向其他应用模块的文件资产业务契约。
/// 调用方不直接依赖 File 模块的 Handler、Writer 或 Deleter。
/// 上传和删除应在实现 IRequireTransaction 的应用请求中调用，以保证数据库与物理文件的补偿流程。
/// </summary>
public interface IFileAssetService
{
    /// <summary>
    /// 创建文件资产并返回其标识。文件元数据与物理文件写入沿用 File 模块的事务补偿机制。
    /// </summary>
    Task<Guid> UploadAsync(
        IFormFile file,
        Guid categoryId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传到 File 模块定义的默认分类，调用方无需依赖 Category 模块的默认分类标识。
    /// </summary>
    Task<Guid> UploadToDefaultCategoryAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布单个文件资产。
    /// </summary>
    Task<Guid> PublishAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量发布文件资产。
    /// </summary>
    Task<int> PublishAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 请求删除文件资产。若任一引用模块仍在使用该文件，则 File 模块会保留文件；物理文件删除由事务管道在数据库事务提交后执行。
    /// </summary>
    Task DeleteAsync(
        IEnumerable<Guid> fileIds,
        CancellationToken cancellationToken = default);
}

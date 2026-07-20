using MyLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using MyLife.Domain.Entities;

namespace MyLife.Service.Interfaces.EntityServices
{
    /// <summary>
    /// 文件业务服务接口
    /// 定义所有文件相关的业务操作
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 查询文件
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="UID">文件 ID（可选）</param>
        /// <param name="Title">文件标题（可选）</param>
        Task<IEnumerable<FileMetaEntity>?> TryReadAsync(CancellationToken ct, Guid? UID = null, string? Title = null);

        /// <summary>
        /// 获取文件下载流
        /// </summary>
        /// <param name="id">文件 ID</param>
        /// <param name="ct">取消令牌</param>
        Task<FileDownloadModel> GetFileInternalAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// 创建文件（上传）
        /// </summary>
        /// <param name="files">文件集合</param>
        /// <param name="ct">取消令牌</param>
        Task<IEnumerable<Guid>> TryCreateAsync(IEnumerable<IFormFile> files, CancellationToken ct);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ids">文件 ID 列表</param>
        /// <param name="ct">取消令牌</param>
        Task TryDeleteAsync(IEnumerable<Guid?> ids, CancellationToken ct);

        /// <summary>
        /// 更新文件标签
        /// </summary>
        /// <param name="guid">文件 ID</param>
        /// <param name="tags">标签 DTO 列表</param>
        /// <param name="ct">取消令牌</param>
        Task TryUpdateAsync(Guid guid, IEnumerable<TagDto> tags, CancellationToken ct);

        /// <summary>
        /// 获取文件 DTO（对外接口）
        /// </summary>
        /// <param name="guid">文件 ID</param>
        /// <param name="ct">取消令牌</param>
        Task<FileDto?> TryGetDtoAsync(Guid guid, CancellationToken ct);
    }
}

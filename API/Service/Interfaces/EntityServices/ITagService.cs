using MyLife.Domain.Entities;
using MyLife.Shared.IView;

namespace MyLife.Service.Interfaces.EntityServices
{
    /// <summary>
    /// 标签业务服务接口
    /// 定义所有标签相关的业务操作
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// 搜索标签
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="guid">标签 ID（可选）</param>
        /// <param name="tag">标签名称（可选，支持模糊匹配）</param>
        Task<IEnumerable<TagEntity?>> Search(CancellationToken ct, Guid? guid = null, string? tag = null);

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="name">标签名称</param>
        /// <param name="aliasName">别名</param>
        /// <param name="isCategory">是否为分类</param>
        Task<Guid> TryCreateAsync(CancellationToken ct, string name, string? aliasName = null, bool isCategory = false);

        /// <summary>
        /// 更新标签
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="guid">标签 ID</param>
        /// <param name="name">标签名称</param>
        /// <param name="aliasName">别名</param>
        /// <param name="isCategory">是否为分类</param>
        Task<Guid> TryUpdateAsync(CancellationToken ct, Guid guid, string name, string? aliasName = null, bool isCategory = false);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="guid">标签 ID</param>
        /// <param name="ct">取消令牌</param>
        Task TryDeleteAsync(Guid guid, CancellationToken ct);

        /// <summary>
        /// 获取标签 DTO（对外接口）
        /// </summary>
        /// <param name="guid">标签 ID</param>
        /// <param name="ct">取消令牌</param>
        Task<TagDto?> TryGetDtoAsync(Guid guid, CancellationToken ct);
    }
}

using MyLife.Domain.Entities;
using MyLife.Shared.IView;

namespace MyLife.Service.Interfaces.EntityServices
{
    /// <summary>
    /// 待办事项业务服务接口
    /// 定义所有待办事项相关的业务操作
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// 获取待办列表
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="status">状态过滤（可选）</param>
        Task<IEnumerable<TodoEntity>?> TryReadListAsync(CancellationToken ct, Guid userId, int? status = null);

        /// <summary>
        /// 获取单个待办事项
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="id">待办 ID</param>
        Task<TodoEntity?> TryReadAsync(CancellationToken ct, Guid id);

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="dto">待办 DTO</param>
        Task<Guid> TryCreateAsync(CancellationToken ct, TodoDto dto);

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="id">待办 ID</param>
        /// <param name="dto">待办 DTO</param>
        Task<Guid> TryUpdateAsync(CancellationToken ct, Guid id, TodoDto dto);

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="id">待办 ID</param>
        Task TryDeleteAsync(CancellationToken ct, Guid id);

        /// <summary>
        /// 完成待办事项
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <param name="id">待办 ID</param>
        Task<Guid> CompleteAsync(CancellationToken ct, Guid id);

        /// <summary>
        /// 获取待办 DTO（对外接口）
        /// </summary>
        /// <param name="id">待办 ID</param>
        /// <param name="ct">取消令牌</param>
        Task<TodoDto?> TryGetDtoAsync(Guid id, CancellationToken ct);
    }
}

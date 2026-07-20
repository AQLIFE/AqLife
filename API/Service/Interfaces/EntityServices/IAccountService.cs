using MyLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using MyLife.Domain.Entities;
using MyLife.Shared;

namespace MyLife.Service.Interfaces.EntityServices
{
    /// <summary>
    /// 账户业务服务接口
    /// 定义所有账户相关的业务操作
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// 获取账户列表
        /// </summary>
        Task<IEnumerable<AccountEntity>> TryReadListAsync();

        /// <summary>
        /// 获取单个账户信息
        /// </summary>
        /// <param name="id">账户 ID</param>
        Task<AccountEntity?> TryReadAsync(Guid? id = null);

        /// <summary>
        /// 创建账户（基础版本）
        /// </summary>
        Task<string> TryCreateAccountAsync(string name, string? desc, string pwd, CancellationToken ct);

        /// <summary>
        /// 创建账户（完整版本，包括文件）
        /// </summary>
        Task<Guid> TryCreateAccountAsync(ISimpleAccountInfo dto, IEnumerable<IFormFile> files, CancellationToken ct);

        /// <summary>
        /// 更新账户基础信息
        /// </summary>
        Task<string> TryUpdateAsync(Guid guid, string name, string? desc, CancellationToken ct);

        /// <summary>
        /// 更新账户头像
        /// </summary>
        Task<string> TryUpdateAsync(Guid guid, IFormFile avatar, CancellationToken ct);

        /// <summary>
        /// 更新账户订阅列表
        /// </summary>
        Task<string> TryUpdateAsync(Guid guid, IEnumerable<SubscriptionDto> dtos, CancellationToken ct);

        /// <summary>
        /// 删除账户
        /// </summary>
        Task TryDeleteAsync(Guid guid, CancellationToken ct);

        /// <summary>
        /// 验证账户登录信息
        /// </summary>
        Task<AccountEntity?> TryValidateAccountAsync(string loginName, string password, CancellationToken ct);

        /// <summary>
        /// 获取账户 DTO（对外接口）
        /// </summary>
        Task<AccountDto?> TryGetDtoAsync(Guid guid, CancellationToken ct);
    }
}

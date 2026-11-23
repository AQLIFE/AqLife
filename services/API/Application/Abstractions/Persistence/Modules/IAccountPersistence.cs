using AqLife.Domain.Entities;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Account 模块专属持久化入口；仅暴露 Account 模块拥有的数据和写操作。
/// </summary>
public interface IAccountPersistence
{
    IQueryable<AccountEntity> Accounts { get; }
    IQueryable<SystemStateEntity> SystemStates { get; }

    Task AddAccountAsync(AccountEntity account, CancellationToken cancellationToken = default);
    void RemoveAccount(AccountEntity account);
    Task AddSystemStateAsync(SystemStateEntity state, CancellationToken cancellationToken = default);
    void AddSubscriptions(IEnumerable<SubscriptionEntity> subscriptions);
    void RemoveSubscriptions(IEnumerable<SubscriptionEntity> subscriptions);
}

using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;

namespace MyLife.Service.StrategiesService
{
    public class SileAccountCheckStrategy(AppStorage storage) : IAccountUploadStrategy
    {
        public ServiceStatus Check(AccountDto account)
        => storage.Account.AsNoTracking().Any(a => a.IsValid)
            ? new ServiceStatus("已存在有效账户，不允许注册", false)
            : new ServiceStatus(string.Empty);
    }
    public class DuplicateNameCheckStrategy(AppStorage storage): IAccountUploadStrategy
    {
        public ServiceStatus Check(AccountDto account)
        => storage.Account.AsNoTracking().Any(a => a.Name == account.Name)
            ? new ServiceStatus("账户名称已存在", false)
            : new ServiceStatus(string.Empty);
    }

    public class AccountSubscriptionCheckStrategy : IAccountUploadStrategy
    {
        public ServiceStatus Check(AccountDto account)
        => account.Subscriptions != null && account.Subscriptions.Any()
            ? new ServiceStatus(string.Empty)
            : new ServiceStatus("至少需要一个订阅", false);
    }

    
    public class AccountAvatarValidCheckStrategy(AppStorage storage): IAccountUpdateStrategy
    {
        public ServiceStatus Check(AccountEntity account)
        => storage.File.AsNoTracking().Any(e=>e.UID == account.Avatar)
           ? new ServiceStatus(string.Empty)
            : new ServiceStatus("头像不存在", false);
    }

    public class AccountSubscriptionAvatarValidCheckStrategy(AppStorage storage) : IAccountUpdateStrategy
    {
        /// <summary>
        /// 实现检查账户的订阅中是否存在无效头像的策略,只处理有效GUID值,如果为null,则skip
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ServiceStatus Check(AccountEntity account)
        {
            var icons = account.Subscriptions.Select(s => s.SubscriptionIcon).Distinct().ToList();// subiicon 存在null 的情况
            if (icons == null || icons.Count == 0)// 这一步基本无法触发, 因为前面已经有订阅检查策略, 但为了保险起见, 还是加上这个判断, 如果没有订阅图标, 则直接返回成功状态
                return new ServiceStatus(string.Empty);
            // 业务场景存在 icon.SubscriptionIcon == null 的情况,所以需要过滤掉 null 的情况,只处理有效的 GUID 值


            if (account.Avatar is Guid avatar) icons.Add(avatar);// 添加 用于更新的ID,在后续检查中,如果存在这个ID,则允许更新,否则不允许

            // 检查icons 这种头像ID是否存在存在, 但因为subIcon 存在 null 情况,所以匹配数量远小于icons.count ; 待处理
            var existCount = storage.File.AsNoTracking().ToList();

            foreach (var icon in icons)
                if (icon is null) continue;
                else if (icon is Guid guid)
                {
                    var isExist= storage.File.AsNoTracking().Any(e => e.UID == guid);
                    if(!isExist)
                    {
                        return new ServiceStatus("订阅头像不存在", false);
                    }
                }else return new ServiceStatus("订阅头像ID无效", false);
            return new ServiceStatus(string.Empty);
        }
    }
}

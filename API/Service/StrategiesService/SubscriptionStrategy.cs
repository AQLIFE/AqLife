using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.StrategiesService
{
    public class ValidityStrategy(AppStorage storage) : ISubscriptionUploadStrategy
    {
        public ServiceStatus Check(IEnumerable<SubscriptionDto> dtos)
        {
            var dict = dtos.Select(e => e.SubscriptionLink).ToArray();
            return storage.Subscription.Any(e => dict.Contains(e.SubscriptionLink))
                ? new ServiceStatus("已存在的记录,无法上传", false) :
                new ServiceStatus(string.Empty);
        }

    }

    public class ValidityIndexStrategy(AppStorage storage) : ISubscriptionUpdateStrategy
    {
        public ServiceStatus Check(IEnumerable<SubscriptionDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var result = storage.File.Any(e => e.UID == dto.SubscriptionIcon) ? new ServiceStatus(string.Empty) : new ServiceStatus("无效的订阅图标", false);
                if (!result.IsValid) return result;
            }
            return new ServiceStatus(string.Empty);
        }
    }
}

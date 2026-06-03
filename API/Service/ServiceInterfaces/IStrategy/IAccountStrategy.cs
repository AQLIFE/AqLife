using MyLife.Data.Entities;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;

namespace MyLife.Service.ServiceInterfaces.IStrategy
{
    public interface IAccountUploadStrategy
    {
        ServiceStatus Check(AccountDto dto);
    }

    public interface ISubscriptionUploadStrategy
    {
        ServiceStatus Check(IEnumerable<SubscriptionDto> dtos);
    }

    public interface IAccountUpdateStrategy
    {
        ServiceStatus Check(AccountEntity entity);
    }
    public interface ISubscriptionUpdateStrategy
    {
        ServiceStatus Check(IEnumerable<SubscriptionDto> dtos);
    }
}

using MyLife.Shared.Contracts;

namespace MyLife.Shared.DTOs
{
    /// <summary>
    /// 作为 Account -  API 结果,不再需要进行校验
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Desc"></param>
    /// <param name="Avatar"></param>
    /// <param name="Subscriptions"></param>
    public record AccountDto(
        string Name,
        string? Desc,
        Guid? Avatar,
        IEnumerable<SubscriptionDto> Subscriptions
        ) : IEntityDto, ISimpleAccountInfo;
}

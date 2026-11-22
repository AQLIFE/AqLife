using AqLife.Shared.IView;

namespace AqLife.Application.Account
{
    public record SubscriptionDto(
    string AliasName,
    string SubscriptionLink,
    string SubscriptionPlatform,
    Guid SubscriptionIcon
    ) : IEntityDto, ISubscription;

    public interface ISubscription
    {
        string AliasName { get; init; }
        string SubscriptionLink { get; init; }
        string SubscriptionPlatform { get; init; }
        Guid SubscriptionIcon { get; init; }
    }
}

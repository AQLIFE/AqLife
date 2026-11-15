using AqLife.Domain.Entities;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.IView;

namespace AqLife.Domain.Command
{
    public record SubscriptionQuery(Guid UID) : IQuery<SubscriptionDto>, IRequireValidEntity<SubscriptionEntity>;
    public record CreateSubscriptionCommand(string AliasName, string SubscriptionLink, string SubscriptionPlatform, Guid SubscriptionIcon) : ISubscription, ICreateCommand;
    public record UpdateSubscriptionCommand(Guid UID) : IUpdateCommand, IRequireValidEntity<SubscriptionEntity>;
    public record DeleteSubscriptionCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<SubscriptionEntity>;
}

using MyLife.Domain.CommandInterface;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;

namespace MyLife.Domain.Command
{
    public record SubscriptionQuery(Guid UID) : IQuery<SubscriptionDto>, IRequireValidEntity<SubscriptionEntity>;
    public record CreateSubscriptionCommand(string AliasName, string SubscriptionLink, string SubscriptionPlatform, Guid SubscriptionIcon) : ISubscription, ICreateCommand;
    public record UpdateSubscriptionCommand(Guid UID) : IUpdateCommand, IRequireValidEntity<SubscriptionEntity>;
    public record DeleteSubscriptionCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<SubscriptionEntity>;
}

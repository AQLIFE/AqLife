using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;

namespace AqLife.Application.Modules.Account
{
    public record SubscriptionQuery(Guid UID) : IQuery<SubscriptionDto>, IRequireValidEntity<SubscriptionEntity>;
    public record CreateSubscriptionCommand(string AliasName, string SubscriptionLink, string SubscriptionPlatform, Guid SubscriptionIcon) : ISubscription, ICreateCommand;
    public record UpdateSubscriptionCommand(Guid UID) : IUpdateCommand, IRequireValidEntity<SubscriptionEntity>;
    public record DeleteSubscriptionCommand(Guid UID) : IDeleteCommand, IRequireValidEntity<SubscriptionEntity>;
}

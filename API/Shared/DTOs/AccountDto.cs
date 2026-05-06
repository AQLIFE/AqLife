using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.DTOs
{
    public record AccountDto(
        string Name,
        IEnumerable<SubscriptionDto> Subscriptions
        );

    public record SubscriptionDto(
        Guid SID,
        string AliasName,
        string SubscriptionLink,
        string SubscriptionPlatform,
        string SubscriptionIcon
        );
}

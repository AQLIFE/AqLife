using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyLife.Shared.IView
{
    [Obsolete("落后的设计")]
    public record SubscriptionFullDto(
    [Required(ErrorMessage ="订阅账户名不能为空")]
    string AliasName,
    [Required(ErrorMessage ="订阅链接不能为空")]
    string SubscriptionLink,
    [Required(ErrorMessage ="订阅平台名不能为空")]
    string SubscriptionPlatform,
    IFormFile? NewIconFile
    ) : SubscriptionDto(AliasName, SubscriptionLink, SubscriptionPlatform, null), IEntityDto;

    public record SubscriptionDto(
    string AliasName,
    string SubscriptionLink,
    string SubscriptionPlatform,
    Guid? SubscriptionIcon
    ) : IEntityDto;
}

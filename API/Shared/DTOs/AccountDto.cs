using System.ComponentModel.DataAnnotations;

namespace MyLife.Shared.DTOs
{
    public record AccountDto(
        [ Required(ErrorMessage = "用户名不能为空")]
        [ StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度需在3-20之间")]
        string Name,
        string? Desc,
        [ Required(ErrorMessage = "个人主页不能存在空订阅")]
        IEnumerable<SubscriptionDto> Subscriptions
        );

    public record SubscriptionDto(
        Guid SID,
        [Required(ErrorMessage ="订阅账户名不能为空")]
        string AliasName,
        [Required(ErrorMessage ="订阅链接不能为空")]
        string SubscriptionLink,
        [Required(ErrorMessage ="订阅平台名不能为空")]
        string SubscriptionPlatform,
        [Required(ErrorMessage ="订阅平台Logo不能为空")]
        string SubscriptionIcon
        );
}

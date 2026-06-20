using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.DTOs
{
    public record SubscriptionFullDto(
    [Required(ErrorMessage ="订阅账户名不能为空")]
    string AliasName,
    [Required(ErrorMessage ="订阅链接不能为空")]
    string SubscriptionLink,
    [Required(ErrorMessage ="订阅平台名不能为空")]
    string SubscriptionPlatform,
    Guid? SubscriptionIcon,
    IFormFile? NewIconFile
    ) : SubscriptionDto(AliasName,SubscriptionLink,SubscriptionPlatform,SubscriptionIcon), IEntityDto;

    public record SubscriptionDto(
    [Required(ErrorMessage ="订阅账户名不能为空")]
    string AliasName,
    [Required(ErrorMessage ="订阅链接不能为空")]
    string SubscriptionLink,
    [Required(ErrorMessage ="订阅平台名不能为空")]
    string SubscriptionPlatform,
    Guid? SubscriptionIcon
    ) : IEntityDto;

}

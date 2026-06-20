using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyLife.Shared.DTOs
{
    public record AccountDto(
        [ Required(ErrorMessage = "用户名不能为空")]
        [ StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度需在3-20之间")]
        string Name,
        string? Desc,
        Guid? Avatar,
        [ Required(ErrorMessage = "个人主页不能存在空订阅")]
        IEnumerable<SubscriptionDto> Subscriptions
        ) : IEntityDto;


    public record AccountFullDto(
        string Name,
        string? Desc,
        IFormFile? Avatar,
        IEnumerable<SubscriptionFullDto> Subscriptions);
}

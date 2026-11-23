using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Account;
/// <summary>更新账户基础信息</summary>
public record UpdateAccountProfileCommand(
    Guid UID,
    [Required, StringLength(15, MinimumLength = 3, ErrorMessage = "账户名称长度必须介于3-15之间")] string Name,
    string? Desc
) : IRequireValidEntity<AccountEntity>, IUpdateCommand<string>;
/// <summary>更新账户订阅列表</summary>
public record UpdateAccountSubscriptionsCommand(Guid UID, IEnumerable<SubscriptionDto> Subscriptions)
    : IUpdateCommand<string>, IRequireValidEntity<AccountEntity>;
/// <summary>更新头像</summary>
public record UpdateAccountAvatarCommand(
    Guid UID,
    [Required(ErrorMessage = "必须上传头像文件")] IFormFile Avatar
) : IUpdateCommand<string>, IHasFormFiles, IRequireValidEntity<AccountEntity>
{
    public IEnumerable<IFormFile> GetFiles() => [Avatar];
}
public record UpdatePasswordCommand(
    Guid UID,
    [property: Required(ErrorMessage = "必须提供")] string OldPassword,
    [property: Required(ErrorMessage = "必须提供"), RegularExpression(pattern: @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,32}$", ErrorMessage = "不规范的新密码")] string NewPassword
) : IRequireValidEntity<AccountEntity>, IUpdateCommand<string>;
public record UpdatePwdCommand(string OldPassword, string NewPassword);

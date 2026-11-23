using AqLife.Application.Abstractions.Commands;
using AqLife.Shared;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Account;
/// <summary>创建账户</summary>
public record CreateAccountCommand(
    [Required, StringLength(32, MinimumLength = 3, ErrorMessage = "账户名称长度必须介于3-32之间")] string Name,
    string Pwd,
    string ServerKey,
    [StringLength(255, ErrorMessage = "个人概述不应超过255个字符")] string Desc
) : ICreateCommand<string>, ISimpleAccountInfo;

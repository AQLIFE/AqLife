using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Account;
public record LoginCommand(
    [Required(ErrorMessage = "必填项:name")] string AccountName,
    [Required(ErrorMessage = "必填项:key")] string SecretKey
) : ICommand<string>;

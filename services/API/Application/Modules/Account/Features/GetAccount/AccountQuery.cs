using AqLife.Application.Abstractions.Commands;
namespace AqLife.Application.Account;
public record AccountQuery(Guid? UID = null) : IQuery<AccountDto>;

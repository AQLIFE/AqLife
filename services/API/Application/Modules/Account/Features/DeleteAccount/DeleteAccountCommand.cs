using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities;
namespace AqLife.Application.Account;
public record DeleteAccountCommand(Guid UID) : IRequireValidEntity<AccountEntity>, IDeleteCommand;

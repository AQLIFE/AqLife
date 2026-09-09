using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using AqLife.Application.Business.Account;

namespace AqLife.Application.Business.Account.Handler
{
    public class CreateAccountHandler(IApplicationDbContext storage, AccountMapper mapper) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            AccountEntity entity = mapper.ToEntity(command);
            await storage.Accounts.AddAsync(entity, ct);
            return entity.LoginName;
        }
    }
}

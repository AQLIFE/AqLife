using MediatR;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Application.Abstractions.Persistence;

namespace MyLife.Application.Business.Account.Handler
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

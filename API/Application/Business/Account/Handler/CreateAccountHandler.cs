using MediatR;
using MyLife.Application.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;

namespace MyLife.Application.Business.Account.Handler
{
    public class CreateAccountHandler(AppStorage storage, AccountMapper mapper) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            AccountEntity entity = mapper.ToEntity(command);
            await storage.Account.AddAsync(entity, ct);
            return entity.LoginName;
        }
    }
}

using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using AqLife.Application.Business.Account.Services;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.Account.Handler
{
    public class CreateAccountHandler(IApplicationDbContext storage, AccountMapper mapper,SystemInitializationService initializationService) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            string? key =await initializationService.GetSystemKey(ct);
            if (command.ServerKey != key)
                throw new RequestCheckException("Invalid system key");
            AccountEntity entity = mapper.ToEntity(command);
            await storage.Accounts.AddAsync(entity, ct);
            await initializationService.VoidSystemKey(command.ServerKey,ct);
            return entity.LoginName;
        }
    }
}

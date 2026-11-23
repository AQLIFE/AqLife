using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Account;
using AqLife.Application.Account.Services;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Account.Handler
{
    public class CreateAccountHandler(IAccountPersistence persistence, ICreateMapper<AccountEntity, CreateAccountCommand> mapper, SystemInitializationService initializationService) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            //    string key = await initializationService.GetSystemKey(ct) ?? throw new RequestCheckException("Invalid system key");            

            AccountEntity entity = mapper.ToEntity(command);
            await persistence.AddAccountAsync(entity, ct);
            await initializationService.VoidSystemKey(command.ServerKey, ct);
            return entity.LoginName;
        }
    }
}

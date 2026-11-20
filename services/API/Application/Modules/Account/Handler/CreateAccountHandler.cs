using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Modules.Account.Services;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Modules.Account.Handler
{
    public class CreateAccountHandler(IApplicationDbContext storage, ICreateMapper<AccountEntity, CreateAccountCommand> mapper, SystemInitializationService initializationService) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            //    string key = await initializationService.GetSystemKey(ct) ?? throw new RequestCheckException("Invalid system key");            

            AccountEntity entity = mapper.ToEntity(command);
            await storage.Accounts.AddAsync(entity, ct);
            await initializationService.VoidSystemKey(command.ServerKey, ct);
            return entity.LoginName;
        }
    }
}

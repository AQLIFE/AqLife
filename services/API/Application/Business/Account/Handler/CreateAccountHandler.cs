using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Account.Services;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Business.Account.Handler
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
